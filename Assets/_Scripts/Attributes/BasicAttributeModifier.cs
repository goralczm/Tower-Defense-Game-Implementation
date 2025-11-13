using System;
using System.Collections.Generic;

namespace Attributes
{
    public class BasicAttributeModifier<TEnum> : AttributeModifier<TEnum> where TEnum : Enum
    {
        private readonly TEnum _type;
        private readonly Func<float, float> _operation;
        private readonly float _originalDuration;

        public BasicAttributeModifier(TEnum type, float duration, Func<float, float> operation) : base(duration)
        {
            _type = type;
            _operation = operation;
            _originalDuration = duration;
        }

        public override void Handle(object sender, AttributeQuery<TEnum> query)
        {
            if (EqualityComparer<TEnum>.Default.Equals(query.Type, _type))
                query.Value = _operation(query.Value);
        }

        public override AttributeModifier<TEnum> Clone()
        {
            var clone = new BasicAttributeModifier<TEnum>(_type, _originalDuration, _operation);

            if (_timer != null)
            {
                var remaining = _timer.Time;
                clone._timer?.Reset(remaining);
            }

            return clone;
        }
    }
}