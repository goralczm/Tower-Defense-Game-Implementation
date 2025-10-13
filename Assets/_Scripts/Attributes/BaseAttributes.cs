using System.Collections.Generic;
using System.Linq;

namespace Attributes
{
    public enum AttributeType
    {
        Health,
        Damage,
    }
    
    [System.Serializable]
    public class BaseAttributes
    {
        public List<AttributeQuery> Queries;

        private Dictionary<AttributeType, AttributeQuery> _queryByType = new();

        public float GetBaseAttribute(AttributeType type)
        {
            if (!_queryByType.TryGetValue(type, out AttributeQuery query))
            {
                query = Queries.First(q => q.Type == type);
                if (query == null)
                    return 0;

                _queryByType.Add(type, query);
            }

            return query.Value;
        }
    }
}
