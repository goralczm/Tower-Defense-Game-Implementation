using UnityEngine;

namespace Towers
{
    public class TowerLevel
    {
        public Sprite Icon;
        public int Cost;
    }

    [CreateAssetMenu(menuName = "Towers/New Tower Data", fileName = "New Tower Data")]
    public class TowerData : ScriptableObject
    {
        public string Description;
        public TowerLevel[] Levels;
    }
}
