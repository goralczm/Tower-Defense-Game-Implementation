using Attributes;
using Core;
using ObjectPooling;
using System;
using System.Collections.Generic;
using System.Linq;
using Targeting;
using UnityEngine;

namespace Towers
{
    public class TowerBehaviour : MonoBehaviour, ITargetable, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private TargetingOptions _targetingOption;

        [Header("References")]
        [SerializeField] private SpriteRenderer _rend;
        [SerializeField] private Inventory.Inventory _inventory;
        [SerializeField] private LineOfSight _lineOfSight;

        private TowerData _towerData;
        private Attributes<TowerAttributes> _attributes;
        private List<IAttackStrategy> _attackStrategies = new();
        private int _level;

        public Action<int> OnTowerLevelChanged;
        public event Action<DamageData> OnDamaged;

        public Attributes<TowerAttributes> Attributes => _attributes;
        public TargetingOptions TargetingOption => _targetingOption;
        public Alignment Alignment => Alignment.Friendly;
        public Transform Transform => transform;
        public int Strength => Mathf.CeilToInt(_attributes.GetAttribute(TowerAttributes.Damage));
        public int TargetingPriority => 0;
        public Inventory.Inventory Inventory => _inventory;
        public TowerData TowerData => _towerData;
        public int DisplayLevel => _level + 1;
        public bool IsMaxLevel => _level == _towerData.Levels.Length - 1;
        public int UpgradeCost => !IsMaxLevel ? _towerData.Levels[_level].Cost : int.MaxValue;
        public int SellRefund => Mathf.FloorToInt(_towerData.Levels.Take(DisplayLevel).Sum(l => l.Cost) / 12f);
        public TowerLevel LevelData => _towerData.Levels[_level];
        public TowerLevel NextLevelData => !IsMaxLevel ? _towerData.Levels[_level + 1] : LevelData;
        public LineOfSight LineOfSight => _lineOfSight;

        public float GetDistance(Vector2 position) => Vector2.Distance(position, transform.position);

        private void Start()
        {
            IDamageable.RecordDamageRequest?.Invoke(this);
        }

        private void OnEnable()
        {
            TowerSelectionController.OnTowerSelected += OnTowerSelected;
        }

        private void OnDisable()
        {
            TowerSelectionController.OnTowerSelected -= OnTowerSelected;
        }

        private void OnTowerSelected(TowerBehaviour tower)
        {
            ToggleLineOfSight(tower == this);
        }

        private void ToggleLineOfSight(bool state)
        {
            _lineOfSight.gameObject.SetActive(state);
            if (state)
                _lineOfSight.GenerateViewMesh();
        }

        public void Setup(TowerData towerData, int level = 0)
        {
            _towerData = towerData;

            _attributes = new(new(), _towerData.Levels[level].BaseAttributes);
            _attributes.OnAttributesChanged += OnAttributesChanged;

            SetLevel(level);
        }

        private void OnAttributesChanged()
        {
            _lineOfSight.SetRadius(_attributes.GetAttribute(TowerAttributes.Range));
            if (_lineOfSight.gameObject.activeSelf)
                _lineOfSight.GenerateViewMesh();
            _inventory.SetCapacity((int)_attributes.GetAttribute(TowerAttributes.InventoryCapacity));
        }

        public void Upgrade()
        {
            if (IsMaxLevel) return;

            SetLevel(_level + 1);
        }

        public void SetLevel(int level)
        {
            if (level > _towerData.Levels.Length - 1) return;

            _level = level;
            _rend.sprite = LevelData.Icon;
            _rend.color = LevelData.Color;
            _attributes.SetBaseAttributes(LevelData.BaseAttributes);

            foreach (var attackStrategy in _attackStrategies)
                attackStrategy.Dispose();

            _attackStrategies = LevelData.AttackStrategies
                .Select(strategy => strategy.Clone())
                .ToList();

            for (int i = 0; i < _attackStrategies.Count; i++)
                _attackStrategies[i].Setup(this, i);

            OnTowerLevelChanged?.Invoke(_level);
        }

        private void Update()
        {
            _attributes.Mediator.Update(Time.deltaTime);

            foreach (var strategy in _attackStrategies)
                strategy.Tick(Time.deltaTime);
        }

        public void TakeDamage(float damage, DamageType[] types, string source)
        {
            OnDamaged?.Invoke(new(
                Mathf.Min(_attributes.GetAttribute(TowerAttributes.Health), damage),
                source,
                _towerData.name,
                types,
                transform.position));

            var modifier = new MathAttributeModifier<TowerAttributes>(TowerAttributes.Health, 0f, MathOperation.Subtract, damage);
            _attributes.Mediator.AddModifier(modifier);

            if (_attributes.GetAttribute(TowerAttributes.Health) <= 0f)
                Die(DeathReason.External);
        }

        public void Die(DeathReason reaseon)
        {
            foreach (var attack in _attackStrategies)
                attack.Dispose();

            foreach (var item in Inventory.Items)
                if (item != null)
                    PickupFactory.CreatePickup(item, (Vector2)transform.position + UnityEngine.Random.insideUnitCircle);

            Destroy(gameObject);
        }
    }
}
