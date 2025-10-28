using Inventory;
using TMPro;
using UnityEngine;
using Utilities.UI;

namespace Towers
{
    public class TowerPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _towerHeader;
        [SerializeField] private TextMeshProUGUI _attributesText;
        [SerializeField] private InventoryDisplay _inventoryDisplay;
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
            if (Input.GetKeyDown(KeyCode.E))
                _tower?.Upgrade();
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
            _attributesText.SetText(_tower.Attributes.GetAttributesDescription());
        }
    }
}
