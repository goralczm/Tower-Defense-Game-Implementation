using ArtificeToolkit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    [System.Serializable]
    public class ProjectileEffectDisplay
    {
        [SerializeReference] public IProjectileEffect Effect;
        public string Description;
    }

    [CreateAssetMenu]
    public class EffectsDisplayDatabase : ScriptableObject
    {
        [ForceArtifice] public List<ProjectileEffectDisplay> EffectDisplays;

        public string GetEffectDescription(IProjectileEffect effect)
        {
            return EffectDisplays.FirstOrDefault(e => e.Effect.GetType().Equals(effect.GetType())).Description;
        }
    }
}
