using System;
using UnityEngine;

namespace Inventory
{
    public class Inventory : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _capacity = 9;

        private IItem[] _items;

        public Action<IItem, int> OnSlotChanged;

        public IItem[] Items => _items;

        private void Awake()
        {
            _items = new IItem[_capacity];
        }

        public bool Add(IItem item)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    _items[i] = item;
                    OnSlotChanged?.Invoke(item, i);
                    return true;
                }
            }

            return false;
        }

        public IItem Swap(IItem item, int index)
        {
            IItem old = _items[index];
            _items[index] = item;
            OnSlotChanged?.Invoke(item, index);

            return old;
        }

        public void Remove(IItem item)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == item)
                {
                    _items[i] = null;
                    OnSlotChanged?.Invoke(null, i);
                    break;
                }
            }
        }

        public IItem Get(int index)
        {
            return _items[index];
        }
    }
}
