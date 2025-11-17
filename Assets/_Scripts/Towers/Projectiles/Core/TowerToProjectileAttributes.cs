using Attributes;

namespace Towers
{
    public static class TowerToProjectileAttributes
    {
        public static BaseAttributes<ProjectileAttributes> GetProjectileBaseAttributes(TowerBehaviour tower)
        {
            return new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Damage, tower.Attributes.GetAttribute(TowerAttributes.Damage))
                .Add(ProjectileAttributes.Range, tower.Attributes.GetAttribute(TowerAttributes.Range))
                .Add(ProjectileAttributes.TickRate, tower.Attributes.GetAttribute(TowerAttributes.RateOfFire))
                .Build();
        }
    }
}
