using BuildingSystem.Core;
using UnityEngine;

namespace BuildingSystem.Buildings
{
    public class ObjectBuilding : IBuilding
    {
        public GameObject Prefab;
        public Sprite Icon;

        public GameObject BuildingPrefab => Prefab;
        public Sprite Sprite => Icon;
        public Color Color => Color.white;
        public bool ShowLineOfSight => false;
        public float LineOfSightRadius => 0;
        public string Name => Prefab.name;
        public string Description => "?";

        public bool CanBuild(ref string reason)
        {
            return true;
        }

        public void OnBuild(GameObject newBuilding)
        {
            //noop
        }
    }
}
