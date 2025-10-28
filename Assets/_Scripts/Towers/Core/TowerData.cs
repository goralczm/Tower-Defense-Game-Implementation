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
    }

    [CreateAssetMenu(menuName = "Towers/New Tower Data", fileName = "New Tower Data")]
    public class TowerData : ScriptableObject
    {
        public string Description;
        public TowerLevel[] Levels;

        [SerializeReference, ForceArtifice] public IAttackStrategy[] AttackStrategies;

        private void OnValidate()
        {
            foreach (var attack in AttackStrategies)
                attack.Validate();
        }
    }
}
