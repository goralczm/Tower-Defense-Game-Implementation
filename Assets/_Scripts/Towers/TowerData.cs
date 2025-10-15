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

    public class TowerData : ScriptableObject
    {
        public string Description;
        public TowerLevel[] Levels;

        [SerializeReference, ForceArtifice] public IAttackStrategy[] AttackStrategies;
    }
}
