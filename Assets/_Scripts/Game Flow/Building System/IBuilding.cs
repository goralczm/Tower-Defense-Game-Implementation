using UnityEngine;

namespace GameFlow
{
    public interface IBuilding
    {
        public GameObject BuildingPrefab { get; }
        public Sprite Sprite { get; }
        public bool ShowLineOfSight { get; }
        public float LineOfSightRange { get; }

        public void Build(GameObject newBuilding);
    }
}
