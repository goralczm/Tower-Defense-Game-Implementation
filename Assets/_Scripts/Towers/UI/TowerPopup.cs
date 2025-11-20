using Attributes;
using Core;
using Currency;
using Inventory;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Extensions;
using Utilities.Text;
using Utilities.UI;

namespace Towers
{
    public class TowerPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _towerHeader;
        [SerializeField] private TextMeshProUGUI _attributesText;
        [SerializeField] private InventoryDisplay _inventoryDisplay;
        [SerializeField] private List<AttackSlot> _attackSlots = new();
        [SerializeField] private UITweener _tweener;

        private TowerBehaviour _tower;

        private void OnEnable()
        {
            TowerSelectionController.OnTowerSelected += SetupPopup;
        }

        private void OnDisable()
        {
            TowerSelectionController.OnTowerSelected -= SetupPopup;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && _tower && !_tower.IsMaxLevel)
            {
                if (Bank.Instance.CanAfford(_tower.UpgradeCost))
                {
                    Bank.Instance.RemoveCurrency(_tower.UpgradeCost);
                    _tower.Upgrade();
                }
                else
                    TextBubble.CreateTextBubble("Cannot affort upgrade", _tower.transform.position, Color.red);
            }

            if (Input.GetKeyDown(KeyCode.S) && _tower)
            {
                Bank.Instance.AddCurrency(_tower.SellRefund);
                _tower.Die(DeathReason.Self);
                TowerSelectionController.OnTowerSelected?.Invoke(null);
            }
        }

        private void SetupPopup(TowerBehaviour tower)
        {
            if (_tower)
            {
                _tower.OnTowerLevelChanged -= TowerLevelChanged;
                _tower.Attributes.OnAttributesChanged -= UpdateDisplay;
            }

            _tower = tower;
            if (_tower)
            {
                _tower.OnTowerLevelChanged += TowerLevelChanged;
                _tower.Attributes.OnAttributesChanged += UpdateDisplay;
                UpdateDisplay();
                _tweener.Show();
            }
            else
                _tweener.Hide();
        }

        private void TowerLevelChanged(int _)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            _inventoryDisplay.SetInventory(_tower.Inventory);
            _towerHeader.SetText($"{_tower.TowerData.name} Level {_tower.Level}");
            if (_tower.IsMaxLevel)
                _attributesText.SetText(_tower.Attributes.GetAttributesDescription());
            else
                _attributesText.SetText(_tower.Attributes.GetComparedAttributesDescription(_tower.NextLevelData.BaseAttributes));

            int inventoryCapacity = (int)_tower.Attributes.GetAttribute(TowerAttributes.InventoryCapacity);
            for (int i = 0; i < _attackSlots.Count; i++)
            {
                if (i < inventoryCapacity)
                {
                    var attack = _tower.LevelData.AttackStrategies[i];

                    _attackSlots[i].Setup(attack.Icon, attack.Name, attack.Description);
                    _attackSlots[i].transform.parent.gameObject.SetActive(true);
                }
                else
                    _attackSlots[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
