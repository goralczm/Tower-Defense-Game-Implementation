using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory _inventory;
        [SerializeField] private List<ItemSlot> _slots;
        [SerializeField] private Transform _draggedItemParent;

        public List<ItemSlot> Slots => new(_slots);

        private void Start()
        {
            SetInventory(_inventory);
        }

        public void SetInventory(Inventory inventory)
        {
            if (_inventory != null)
            {
                _inventory.OnSlotChanged -= UpdateSlot;
                _inventory.OnCapacityChanged -= UpdateDisplay;
            }

            _inventory = inventory;
            if (_inventory != null)
            {
                _inventory.OnSlotChanged += UpdateSlot;
                _inventory.OnCapacityChanged += UpdateDisplay;
                UpdateDisplay();
            }
            else
                ClearDisplay();
        }

        public void UpdateDisplay()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].Init(_inventory, _draggedItemParent);

                if (i < _inventory.Capacity)
                    UpdateSlot(_inventory.Items[i], i);
                else
                    UpdateSlot(null, i);

                _slots[i].gameObject.SetActive(i < _inventory.Capacity);
            }
        }

        private void UpdateSlot(IItem item, int index)
        {
            _slots[index].Setup(item, index);
        }

        private void ClearDisplay()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].Init(null, _draggedItemParent);
                UpdateSlot(null, i);
            }
        }
    }
}
