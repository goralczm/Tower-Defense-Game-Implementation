using ArtificeToolkit.Attributes;
using Attributes;
using UnityEngine;

namespace Towers
{
    [System.Serializable]
    public class TowerLevel
    {
        public Sprite Icon;
        public int Cost;
        public BaseAttributes<TowerAttributes> BaseAttributes;
        [SerializeReference, ForceArtifice] public IAttackStrategy[] AttackStrategies;
    }

    [CreateAssetMenu(menuName = "Towers/New Tower Data", fileName = "New Tower Data")]
    public class TowerData : ScriptableObject
    {
        public string Description;
        [ForceArtifice] public TowerLevel[] Levels;

        private void OnValidate()
        {
            foreach (var level in Levels)
                foreach (var attack in level.AttackStrategies)
                    attack?.Validate();
        }
    }
}
