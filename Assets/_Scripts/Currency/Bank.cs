using System;
using UnityEngine;
using Utilities;

namespace Currency
{
    public class Bank : Singleton<Bank>
    {
        [Header("Settings")]
        public int StartCurrency = 4200;

        private int _currency;

        public static event Action<int> OnCurrencyChanged;

        public int Currency => _currency;

        public bool CanAfford(int amount)
        {
            return _currency >= amount;
        }
        
        public void SetCurrency(int currency)
        {
            _currency = currency;
            OnCurrencyChanged?.Invoke(_currency);
        }

        public void AddCurrency(int amount)
        {
            SetCurrency(_currency + amount);
        }

        public void RemoveCurrency(int amount)
        {
            AddCurrency(-amount);
        }
    }
}
