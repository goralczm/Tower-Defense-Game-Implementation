using UnityEngine;

namespace BuildingSystem.Core
{
    public interface IBuilding
    {
        public string Name { get; }
        public string Description { get; }
        public GameObject BuildingPrefab { get; }
        public Sprite Sprite { get; }
        public bool ShowLineOfSight { get; }
        public float LineOfSightRadius { get; }

        public bool CanBuild(ref string reason);
        public void OnBuild(GameObject newBuilding);
    }
}
