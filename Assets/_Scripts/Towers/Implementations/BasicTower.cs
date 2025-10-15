using UnityEngine;

namespace Towers
{
    [CreateAssetMenu(menuName = "Towers/Basic Tower", fileName = "New Basic Tower")]
    public class BasicTower : TowerData
    {
        public ProjectileBase Projectile;

        public override IAttackStrategy[] AttackStrategies => new IAttackStrategy[1]{
            new ShootStrategy(Projectile),
        };
    }
}
