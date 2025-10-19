using Attributes;
using Core;
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

        [Header("Debug")]
        [SerializeField] private TowerData _startTowerData;

        private TowerData _towerData;
        private Attributes<TowerAttributes> _attributes;
        private IAttackStrategy[] _attackStrategies;

        public Attributes<TowerAttributes> Attributes => _attributes;
        public TargetingOptions TargetingOption => _targetingOption;
        public Alignment Alignment => Alignment.Friendly;
        public Transform Transform => transform;
        public int Strength => Mathf.CeilToInt(_attributes.GetAttribute(TowerAttributes.Damage));
        public int Priority => 0;

        public float GetDistance(Vector2 position) => Vector2.Distance(position, transform.position);

        private void Awake()
        {
            if (_startTowerData)
                Setup(_startTowerData, 0);
        }

        public void Setup(TowerData towerData, int level = 0)
        {
            _towerData = towerData;

            _attributes = new(new(), _towerData.Levels[level].BaseAttributes);

            _attackStrategies = _towerData.AttackStrategies
                .Select(strategy => strategy.Clone())
                .ToArray();

            foreach (var strategy in _attackStrategies)
                strategy.Setup(this);

            SetLevel(level);
        }

        public void SetLevel(int level)
        {
            _rend.sprite = _towerData.Levels[level].Icon;
            _attributes.SetBaseAttributes(_towerData.Levels[level].BaseAttributes);
        }

        private void Update()
        {
            foreach (var strategy in _attackStrategies)
                strategy.Tick(Time.deltaTime);
        }

        public void TakeDamage(float damage)
        {
            var modifier = new BasicAttributeModifier<TowerAttributes>(TowerAttributes.Health, 0f, v => v - damage);

            _attributes.Mediator.AddModifier(modifier);

            if (_attributes.GetAttribute(TowerAttributes.Health) <= 0f)
                Die();
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
