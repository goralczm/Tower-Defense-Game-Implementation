using System.Collections.Generic;
using System;
using System.Linq;

namespace Attributes
{
    public class BaseAttributesBuilder<TEnum> where TEnum : Enum
    {
        private readonly List<AttributeQuery<TEnum>> _queries = new();
        private readonly BaseAttributes<TEnum> _baseAttributes;

        public BaseAttributesBuilder() { }

        public BaseAttributesBuilder(BaseAttributes<TEnum> baseAttributes)
        {
            _baseAttributes = baseAttributes;
        }

        public BaseAttributesBuilder<TEnum> Add(TEnum type, float value)
        {
            var newAttribute = new AttributeQuery<TEnum>(type, value);

            var existing = _queries.FirstOrDefault(q => EqualityComparer<TEnum>.Default.Equals(q.Type, type));
            if (existing != null)
                existing += newAttribute;
            else
                _queries.Add(newAttribute);

            return this;
        }

        public BaseAttributesBuilder<TEnum> Set(TEnum type, float value)
        {
            var existing = _queries.FirstOrDefault(q => EqualityComparer<TEnum>.Default.Equals(q.Type, type));
            if (existing != null)
                _queries.Remove(existing);

            _queries.Add(new AttributeQuery<TEnum>(type, value));
            return this;
        }

        public BaseAttributes<TEnum> Build()
        {
            var result = new BaseAttributes<TEnum>
            {
                Queries = _queries
            };

            if (_baseAttributes != null)
                result += _baseAttributes;

            return result;
        }
    }

}
