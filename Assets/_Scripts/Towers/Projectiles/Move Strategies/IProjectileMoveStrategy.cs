using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public interface IProjectileMoveStrategy
    {
        public bool DestroyIfInvalidTarget { get; }

        public event Action<Transform> OnTransformCollision;

        public void Init(ProjectileBehaviour projectile);
        public void Move(Vector2 target);
        public IProjectileMoveStrategy Clone();
    }
}
