using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Attributes
{
    [System.Serializable]
    public class BaseAttributes<TEnum> where TEnum : Enum
    {
        public List<AttributeQuery<TEnum>> Queries;

        private Dictionary<TEnum, AttributeQuery<TEnum>> _queryByType = new();

        public float GetBaseAttribute(TEnum type)
        {
            if (!_queryByType.TryGetValue(type, out AttributeQuery<TEnum> query))
            {
                query = Queries.First(q => EqualityComparer<TEnum>.Default.Equals(q.Type, type));
                if (query == null)
                    return 0;

                _queryByType.Add(type, query);
            }

            return query.Value;
        }
    }
}
