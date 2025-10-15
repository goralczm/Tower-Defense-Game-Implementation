using System;
using System.Collections.Generic;

namespace Attributes
{
    public class BasicAttributeModifier<TEnum> : AttributeModifier<TEnum> where TEnum : Enum
    {
        private readonly TEnum _type;
        private readonly Func<float, float> _operation;

        public BasicAttributeModifier(TEnum type, float duration, Func<float, float> operation) : base(duration)
        {
            _type = type;
            _operation = operation;
        }

        public override void Handle(object sender, AttributeQuery<TEnum> query)
        {
            if (EqualityComparer<TEnum>.Default.Equals(query.Type, _type))
                query.Value = _operation(query.Value);
        }
    }
}