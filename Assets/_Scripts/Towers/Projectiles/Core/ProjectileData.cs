using ArtificeToolkit.Attributes;
using Attributes;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Towers.Projectiles
{
    [CreateAssetMenu(menuName = "Projectiles/New Projectile Data", fileName = "New Projectile Data")]
    public class ProjectileData : ScriptableObject
    {
        public Sprite Sprite;
        public Color Color;
        public BaseAttributes<ProjectileAttributes> BaseAttributes;
        [SerializeReference, ForceArtifice] public List<IProjectileDamageStrategy> DamageStrategies;

        public void Randomazzo()
        {
            BaseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Speed, Random.Range(5f, 6f))
                .Add(ProjectileAttributes.Size, Random.Range(.2f, 1f))
                .Add(ProjectileAttributes.Range, 10f)
                .Add(ProjectileAttributes.Bounces, 6)
                .Build();
            Color = Randomizer.GetRandomColor();
        }
    }
}
