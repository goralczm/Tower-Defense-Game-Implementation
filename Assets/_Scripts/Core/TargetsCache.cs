using System.Collections.Generic;
using UnityEngine;

namespace Core.Cache
{
    public static class TargetsCache
    {
        private static Dictionary<Collider2D, ITargetable> _targetsByCollider = new();

        public static bool TryGetTarget(Collider2D collider, out ITargetable target)
        {
            target = GetTargetByCollider(collider);

            if (target == null)
                return false;

            return true;
        }

        public static ITargetable GetTargetByCollider(Collider2D collider)
        {
            if (!_targetsByCollider.TryGetValue(collider, out ITargetable target))
            {
                target = collider.GetComponent<ITargetable>();
                _targetsByCollider.Add(collider, target);
            }

            return target;
        }
    }
}
