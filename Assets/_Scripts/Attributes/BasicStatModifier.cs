using System;

namespace Attributes
{
    public class BasicStatModifier : AttributeModifier
    {
        private readonly AttributeType _type;
        private readonly Func<float, float> _operation;

        public BasicStatModifier(AttributeType type, float duration, Func<float, float> operation) : base(duration)
        {
            _type = type;
            _operation = operation;
        }

        public override void Handle(object sender, AttributeQuery query)
        {
            if (query.Type == _type)
                query.Value = _operation(query.Value);
        }
    }
}