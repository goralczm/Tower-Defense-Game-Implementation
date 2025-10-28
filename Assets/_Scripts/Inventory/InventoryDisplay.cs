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

        private void Start()
        {
            SetInventory(_inventory);
        }

        public void SetInventory(Inventory inventory)
        {
            if (_inventory != null)
                _inventory.OnSlotChanged -= UpdateSlot;

            _inventory = inventory;
            if (_inventory != null)
            {
                _inventory.OnSlotChanged += UpdateSlot;
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

                if (_inventory.Items != null)
                    UpdateSlot(_inventory.Items[i], i);
                else
                    UpdateSlot(null, i);
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
