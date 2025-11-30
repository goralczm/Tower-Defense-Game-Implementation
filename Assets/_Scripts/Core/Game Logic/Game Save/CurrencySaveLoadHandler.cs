using Currency;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.GameSave
{
    public class CurrencySaveLoadHandler : SaveLoadHandler
    {
        [Header("Refereneces")]
        [SerializeField] private Bank _bank;

        public override async Task Load()
        {
            int? savedCurrency = SaveSystem.SaveSystem.LoadData<int?>(SaveKey) as int?;

            if (savedCurrency == null)
                await FallbackSetup?.Setup();
            else
                _bank.SetCurrency(savedCurrency.Value);
        }

        public override void Save()
        {
            int currency = _bank.Currency;

            SaveSystem.SaveSystem.SaveData(currency, SaveKey);
        }
    }
}
