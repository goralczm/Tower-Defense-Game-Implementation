using System.Collections.Generic;
using UnityEngine;

namespace Core.Cache
{
    public static class TargetsCache
    {
        private static Dictionary<Transform, ITargetable> _targetsByTransform = new();

        public static bool TryGetTarget(Collider2D collider, out ITargetable target)
        {
            target = GetTargetByTransform(collider.transform);

            if (target == null)
                return false;

            return true;
        }

        public static bool TryGetTarget(Transform transform, out ITargetable target)
        {
            target = GetTargetByTransform(transform);

            if (target == null)
                return false;

            return true;
        }

        public static ITargetable GetTargetByTransform(Transform transform)
        {
            if (!_targetsByTransform.TryGetValue(transform, out ITargetable target))
            {
                target = transform.GetComponent<ITargetable>();
                _targetsByTransform.Add(transform, target);
            }

            return target;
        }
    }
}
