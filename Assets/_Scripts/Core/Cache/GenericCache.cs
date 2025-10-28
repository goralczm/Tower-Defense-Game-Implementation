using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class GenericCache<T>
    {
        private Dictionary<object, T> _cache = new();

        public bool TryGetKey<V>(V key, out T value) where V : Component
        {
            if (!_cache.TryGetValue(key, out value))
            {
                value = key.GetComponent<T>();
                if (value != null)
                    _cache.Add(key, value);

                return value != null;
            }

            return true;
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
