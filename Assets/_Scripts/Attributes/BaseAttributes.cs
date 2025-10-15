using System;
using System.Collections.Generic;
using System.Linq;

namespace Attributes
{
    [System.Serializable]
    public class BaseAttributes<TEnum> where TEnum : Enum
    {
        public List<AttributeQuery<TEnum>> Queries = new();

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

        public static BaseAttributes<TEnum> operator +(BaseAttributes<TEnum> a, BaseAttributes<TEnum> b)
        {
            var resultDict = new Dictionary<TEnum, float>();

            foreach (var q in a.Queries)
                resultDict[q.Type] = q.Value;

            foreach (var q in b.Queries)
            {
                if (resultDict.TryGetValue(q.Type, out var val))
                    resultDict[q.Type] = val + q.Value;
                else
                    resultDict[q.Type] = q.Value;
            }

            var combined = resultDict
                .Select(kvp => new AttributeQuery<TEnum>(kvp.Key, kvp.Value))
                .ToList();

            var result = new BaseAttributes<TEnum>
            {
                Queries = combined
            };

            return result;
        }
    }
}
