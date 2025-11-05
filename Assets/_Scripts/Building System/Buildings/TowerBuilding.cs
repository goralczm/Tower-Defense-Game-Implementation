using BuildingSystem.Core;
using Currency;
using Towers;
using UnityEngine;

namespace BuildingSystem.Buildings
{
    public class TowerBuilding : IBuilding
    {
        public TowerData Tower;
        public GameObject Prefab;

        public GameObject BuildingPrefab => Prefab;
        public Sprite Sprite => Tower.Levels[0].Icon;
        public bool ShowLineOfSight => true;
        public float LineOfSightRadius => Tower.Levels[0].BaseAttributes.GetAttribute(Attributes.TowerAttributes.Range);
        public string Name => Tower.name;
        public string Description => Tower.Description;

        public bool CanBuild(ref string reason)
        {
            if (Bank.Instance.CanAfford(Tower.Levels[0].Cost))
                return true;

            reason = "Not enough currency";
            return false;
        }

        public void OnBuild(GameObject newBuilding)
        {
            Bank.Instance.RemoveCurrency(Tower.Levels[0].Cost);

            newBuilding.GetComponent<TowerBehaviour>().Setup(Tower);
        }
    }
}
