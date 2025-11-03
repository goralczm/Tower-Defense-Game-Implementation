using Towers;
using UnityEngine;

namespace GameFlow
{
    public class TowerBuilding : IBuilding
    {
        public TowerData Tower;
        public GameObject Prefab;

        public GameObject BuildingPrefab => Prefab;
        public Sprite Sprite => Tower.Levels[0].Icon;
        public bool ShowLineOfSight => true;
        public float LineOfSightRadius => Tower.Levels[0].BaseAttributes.GetAttribute(Attributes.TowerAttributes.Range);

        public void Build(GameObject newBuilding)
        {
            newBuilding.GetComponent<TowerBehaviour>().Setup(Tower);
        }
    }
}
