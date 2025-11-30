using Inventory;
using SaveSystem;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.GameSave
{
    public class InventorySaveLoadHandler : SaveLoadHandler
    {
        [Header("References")]
        [SerializeField] private Inventory.Inventory _inventory;

        public override async Task Load()
        {
            SaveableData savedItems = SaveSystem.SaveSystem.LoadData<SaveableData>(SaveKey) as SaveableData;
            if (savedItems == null)
            {
                await FallbackSetup?.Setup();
                Debug.Log("No inventory data saved");
                return;
            }

            object savedObject = savedItems.GetObject("Item Ids");

            if (savedObject == null)
                return;

            int?[] saveDatas = (int?[])savedObject;

            var allItems = Resources.LoadAll<ItemBase>("Items");

            for (int i = 0; i < saveDatas.Length; i++)
            {
                int? itemId = saveDatas[i];

                if (itemId != null)
                    _inventory.Swap(allItems.FirstOrDefault(i => i.Id == itemId), i);
            }
        }

        public override void Save()
        {
            SaveableData savedItems = new();

            var items = _inventory.Items;
            int?[] itemIds = new int?[items.Count];

            for (int i = 0; i < items.Count; i++)
                itemIds[i] = items[i]?.Id;

            savedItems.SaveObject("Item Ids", itemIds);

            SaveSystem.SaveSystem.SaveData(savedItems, SaveKey);
        }
    }
}
