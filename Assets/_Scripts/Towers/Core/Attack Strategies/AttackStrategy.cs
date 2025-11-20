using UnityEngine;

namespace Towers
{
    public abstract class AttackStrategy : ScriptableObject, IAttackStrategy
    {
        public string AttackDescription;
        public Sprite AttackIcon;

        public string Name => name;
        public string Description => AttackDescription;
        public Sprite Icon => AttackIcon;

        public abstract IAttackStrategy Clone();

        public abstract void Dispose();

        public abstract void Setup(TowerBehaviour tower, int index);

        public abstract void Tick(float deltaTime);

        public abstract void Validate();
    }
}
