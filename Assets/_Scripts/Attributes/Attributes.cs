using System;

namespace Attributes
{
    [System.Serializable]
    public class Attributes<TEnum> where TEnum : Enum
    {
        private readonly AttributesMediator<TEnum> _mediator;

        private BaseAttributes<TEnum> _baseAttributes;

        public AttributesMediator<TEnum> Mediator => _mediator;

        public Attributes(AttributesMediator<TEnum> mediator, BaseAttributes<TEnum> baseAttributes)
        {
            _mediator = mediator;
            _baseAttributes = baseAttributes;
        }

        public float GetAttribute(TEnum type)
        {
            var q = new AttributeQuery<TEnum>(type, _baseAttributes.GetBaseAttribute(type));
            _mediator.PerformQuery(this, q);

            return q.Value;
        }

        public void SetBaseAttributes(BaseAttributes<TEnum> baseAttributes)
        {
            _baseAttributes = baseAttributes;
        }
    }
}