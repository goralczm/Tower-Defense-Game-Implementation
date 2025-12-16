using ArtificeToolkit.Attributes;
using Attributes;
using Core;
using Inventory;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utilities;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
}

namespace Towers.Projectiles
{
    [CreateAssetMenu(menuName = "Projectiles/New Projectile Data", fileName = "New Projectile Data")]
    public class ProjectileData : ItemBase
    {
        public int UniqueId;
        public Sprite Sprite;
        public Color SpriteColor;
        public Rarity Rarity;
        public BaseAttributes<ProjectileAttributes> BaseAttributes;
        [SerializeReference, ForceArtifice] public IProjectileMovement MoveStrategy;
        [SerializeReference, ForceArtifice] public List<IProjectileEffect> DamageStrategies;
        public DamageType[] DamageTypes;

        public override int Id => UniqueId;
        public override string Name => $"{name} <size=75%>({GetDamageTypesString()})</size>";
        public override string Description => GetDescription();
        public override Sprite Icon => Sprite;
        public override Color Color => SpriteColor;

        private static Dictionary<int, string> _cachedDescription = new();

        public void Randomazzo()
        {
            BaseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Speed, Random.Range(5f, 15f))
                .Add(ProjectileAttributes.Size, Random.Range(.5f, 1.5f))
                .Add(ProjectileAttributes.Range, 10f)
                .Build();
            SpriteColor = Randomizer.GetRandomColor();
        }

        public string GetDamageTypesString()
        {
            StringBuilder output = new();

            for (int i = 0; i < DamageTypes.Length; i++)
            {
                output.Append(DamageTypes[i].ToString());
                if (i < DamageTypes.Length - 1)
                    output.Append(", ");
            }

            return output.ToString();
        }

        public string GetDescription()
        {
            if (!_cachedDescription.TryGetValue(Id, out var description))
            {
                EffectsDisplayDatabase effectsDisplayDatabase = Resources.Load<EffectsDisplayDatabase>("Effects Display Database");

                StringBuilder output = new();

                string attributesDescription = new Attributes<ProjectileAttributes>(new(), BaseAttributes).GetAttributesDescription();

                output.Append(attributesDescription);
                output.Append("Effects:");

                foreach (var effect in DamageStrategies)
                {
                    output.Append("\n - ");
                    output.Append(effectsDisplayDatabase.GetEffectDescription(effect));
                }

                description = output.ToString();
                _cachedDescription[Id] = description;
            }

            return description;
        }
    }
}
