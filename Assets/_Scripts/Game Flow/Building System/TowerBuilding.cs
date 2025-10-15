using Towers;
using UnityEngine;

namespace GameFlow
{
    [CreateAssetMenu(menuName = "Buildings/Tower Building", fileName = "New Tower Building")]
    public class TowerBuilding : ScriptableObject, IBuilding
    {
        public TowerData Tower;
        public GameObject Prefab;

        public GameObject BuildingPrefab => Prefab;
        public Sprite Sprite => Tower.Levels[0].Icon;
        public bool ShowLineOfSight => true;
        public float LineOfSightRange => Tower.Levels[0].BaseAttributes.GetBaseAttribute(Attributes.TowerAttributes.Range);

        public void Build(GameObject newBuilding)
        {
            newBuilding.GetComponent<TowerBehaviour>().Setup(Tower);
        }
    }
}
