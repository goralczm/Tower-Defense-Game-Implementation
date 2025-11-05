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
        public bool ShowLineOfSight => false;
        public float LineOfSightRadius => 0;

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
