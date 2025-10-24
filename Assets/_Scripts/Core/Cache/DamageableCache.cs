using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public static class DamageableCache
    {
        private static Dictionary<Transform, IDamageable> _damageablesByTransform = new();

        public static bool TryGetDamageable(Transform transform, out IDamageable target)
        {
            target = GetDamageableByTransform(transform);

            if (target == null)
                return false;

            return true;
        }

        public static IDamageable GetDamageableByTransform(Transform transform)
        {
            if (!_damageablesByTransform.TryGetValue(transform, out IDamageable damageable))
            {
                damageable = transform.GetComponent<IDamageable>();
                _damageablesByTransform.Add(transform, damageable);
            }

            return damageable;
        }
    }
}
