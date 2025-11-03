using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Attributes
{
    [System.Serializable]
    public class Attributes<TEnum> where TEnum : Enum
    {
        private readonly AttributesMediator<TEnum> _mediator;

        private BaseAttributes<TEnum> _baseAttributes;

        public event Action OnAttributesChanged;

        public AttributesMediator<TEnum> Mediator => _mediator;

        public Attributes(AttributesMediator<TEnum> mediator, BaseAttributes<TEnum> baseAttributes)
        {
            _mediator = mediator;
            _baseAttributes = baseAttributes;
            _mediator.OnAttributesChanged += () => OnAttributesChanged?.Invoke();
        }

        public float GetAttribute(TEnum type, float defaultValue = 0f)
        {
            var q = new AttributeQuery<TEnum>(type, _baseAttributes.GetAttribute(type, defaultValue));
            _mediator.PerformQuery(this, q);

            return q.Value;
        }

        public void SetBaseAttributes(BaseAttributes<TEnum> baseAttributes)
        {
            _baseAttributes = baseAttributes;
        }

        public IEnumerable<KeyValuePair<TEnum, float>> GetAllAttributes()
        {
            foreach (TEnum type in Enum.GetValues(typeof(TEnum)))
            {
                float value = GetAttribute(type);
                if (value != 0)
                    yield return new KeyValuePair<TEnum, float>(type, value);
            }
        }

        public string GetAttributesDescription()
        {
            var attributes = GetAllAttributes();
            StringBuilder description = new();
            foreach (var attribute in attributes)
            {
                description.Append(attribute.Key);
                description.Append(" ");
                description.Append(attribute.Value);
                description.Append("\n");
            }

            return description.ToString();
        }
    }
}