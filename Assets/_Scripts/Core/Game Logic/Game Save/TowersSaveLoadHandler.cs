using ObjectPooling;
using SaveSystem;
using System.Linq;
using System.Threading.Tasks;
using Towers;
using Towers.Projectiles;
using UnityEngine;
using Utilities.Extensions;

namespace Core.GameSave
{
    [System.Serializable]
    public class TowerSaveData
    {
        public int TowerId;
        public float[] Position;
        public int Level;
        public int?[] ProjectileIds;
        public int TargetingOption;
    }

    public class TowersSaveLoadHandler : SaveLoadHandler
    {
        public override async Task Load()
        {
            SaveableData towersData = SaveSystem.SaveSystem.LoadData<SaveableData>(SaveKey) as SaveableData;
            if (towersData == null)
            {
                Debug.Log("No towers data saved");
                return;
            }

            object savedObject = towersData.GetObject("Tower Save Datas");

            if (savedObject == null)
                return;

            TowerSaveData[] saveDatas = (TowerSaveData[])savedObject;

            var allTowers = Resources.LoadAll<TowerData>("Towers");
            var allProjectiles = Resources.LoadAll<ProjectileData>("Items/Projectiles");

            foreach (var savedTower in saveDatas)
            {
                var tower = PoolManager.Instance.SpawnFromPool("Tower", savedTower.Position.ToVector3(), Quaternion.identity);
                var towerData = allTowers.First(t => t.Id == savedTower.TowerId);

                var towerBehaviour = tower.GetComponent<TowerBehaviour>();
                towerBehaviour.Setup(towerData);
                towerBehaviour.SetLevel(savedTower.Level);

                for (int i = 0; i < savedTower.ProjectileIds.Length; i++)
                {
                    int? savedItemId = savedTower.ProjectileIds[i];

                    if (savedItemId != null)
                        towerBehaviour.Inventory.Swap(allProjectiles.FirstOrDefault(p => p.Id == savedItemId), i);
                }
            }
        }

        public override void Save()
        {
            TowerBehaviour[] towers = FindObjectsByType<TowerBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

            SaveableData towersData = new();
            TowerSaveData[] saveDatas = new TowerSaveData[towers.Length];

            for (int i = 0; i < towers.Length; i++)
            {
                TowerBehaviour tower = towers[i];

                TowerSaveData saveData = new()
                {
                    TowerId = tower.TowerData.Id,
                    Position = tower.transform.position.ToFloat3(),
                    Level = tower.DisplayLevel - 1,
                    ProjectileIds = tower.Inventory.Items.Select(i => i?.Id).ToArray(),
                    TargetingOption = (int)tower.TargetingOption,
                };

                saveDatas[i] = saveData;
            }

            towersData.SaveObject("Tower Save Datas", saveDatas);

            SaveSystem.SaveSystem.SaveData(towersData, SaveKey);
        }
    }
}
