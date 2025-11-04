using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class Inventory : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _capacity = 9;

        private List<IItem> _items;

        public Action<IItem, int> OnSlotChanged;
        public Action OnCapacityChanged;

        public List<IItem> Items => _items;

        public int Capacity => _capacity;

        public void SetCapacity(int capacity)
        {
            if (capacity > _capacity)
            {
                for (int i = 0; i < capacity - _capacity; i++)
                {
                    _items.Add(null);
                    OnSlotChanged?.Invoke(null, _items.Count - 1);
                }
            }
            else
            {
                for (int i = 0; i < _capacity - capacity; i++)
                {
                    _items.RemoveAt(_items.Count - 1 - i);
                    OnSlotChanged?.Invoke(null, _items.Count - 1 - i);
                }
            }

            _capacity = capacity;
            OnCapacityChanged?.Invoke();
        }

        private void Awake()
        {
            _items = new();
            for (int i = 0; i < _capacity; i++)
                _items.Add(null);
        }

        public bool Add(IItem item)
        {
            for (int i = 0; i < _items.Count; i++)
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
            for (int i = 0; i < _items.Count; i++)
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
