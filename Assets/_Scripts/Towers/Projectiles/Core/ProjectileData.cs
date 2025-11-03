using ArtificeToolkit.Attributes;
using Attributes;
using Inventory;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Towers.Projectiles
{
    [CreateAssetMenu(menuName = "Projectiles/New Projectile Data", fileName = "New Projectile Data")]
    public class ProjectileData : ScriptableObject, IItem
    {
        public Sprite Sprite;
        public Color SpriteColor;
        public BaseAttributes<ProjectileAttributes> BaseAttributes;
        [SerializeReference, ForceArtifice] public IProjectileMoveStrategy MoveStrategy;
        [SerializeReference, ForceArtifice] public List<IProjectileDamageStrategy> DamageStrategies;

        public string Name => name;
        public string Description => "Not set yet.";
        public Sprite Icon => Sprite;
        public Color Color => SpriteColor;

        public void Randomazzo()
        {
            BaseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Speed, Random.Range(5f, 15f))
                .Add(ProjectileAttributes.Size, Random.Range(.5f, 1.5f))
                .Add(ProjectileAttributes.Range, 10f)
                .Build();
            SpriteColor = Randomizer.GetRandomColor();
        }
    }
}
