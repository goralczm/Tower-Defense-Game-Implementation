using UnityEngine;

namespace Core
{
    public interface ITargetable
    {
        public Alignment Alignment { get; }

        public Transform Transform { get; }

        public int Strength { get; }

        public int Priority { get; }

        public float GetDistance(Vector2 position);
    }
}
