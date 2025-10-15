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

    public abstract class TowerData : ScriptableObject
    {
        public string Description;
        public TowerLevel[] Levels;

        public virtual IAttackStrategy[] AttackStrategies => new IAttackStrategy[0];
    }
}
