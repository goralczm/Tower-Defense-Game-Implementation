using System;
using System.Collections.Generic;

namespace Attributes
{
    public class FunctionAttributeModifier<TEnum> : AttributeModifier<TEnum> where TEnum : Enum
    {
        public TEnum Type;
        public Func<float, float> Operation;
        public float OriginalDuration;

        public FunctionAttributeModifier(TEnum type, float duration, Func<float, float> operation) : base(duration)
        {
            Type = type;
            Operation = operation;
            OriginalDuration = duration;
        }

        public override void Handle(object sender, AttributeQuery<TEnum> query)
        {
            if (EqualityComparer<TEnum>.Default.Equals(query.Type, Type))
                query.Value = Operation(query.Value);
        }

        public override AttributeModifier<TEnum> Clone()
        {
            var clone = new FunctionAttributeModifier<TEnum>(Type, OriginalDuration, Operation);
            if (Timer != null)
            {
                var remaining = Timer.Time;
                clone.Timer?.Reset(remaining);
            }

            return clone;
        }
    }
}
