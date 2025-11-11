using System;
using UnityEngine;
using Utilities;

namespace Currency
{
    public class Bank : Singleton<Bank>
    {
        [SerializeField] private int _currency;

        public static event Action<int> OnCurrencyChanged;

        public int Currency => _currency;

        private void Start()
        {
            OnCurrencyChanged?.Invoke(_currency);
        }

        public bool CanAfford(int amount)
        {
            return _currency >= amount;
        }

        public void AddCurrency(int amount)
        {
            _currency += amount;

            OnCurrencyChanged?.Invoke(_currency);
        }

        public void RemoveCurrency(int amount)
        {
            AddCurrency(-amount);
        }
    }
}
