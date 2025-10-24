using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public interface IProjectileMoveStrategy
    {
        public void Init(ProjectileBehaviour projectile);
        public void Move(Vector2 target);

        public event Action<Transform> OnTransformCollision;
        public bool DestroyIfInvalidTarget { get; }
        public IProjectileMoveStrategy Clone();
    }
}
