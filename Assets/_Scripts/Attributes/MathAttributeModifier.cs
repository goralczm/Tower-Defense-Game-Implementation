using System;
using System.Collections.Generic;

namespace Attributes
{
    public enum MathOperation
    {
        Override,
        Add,
        Multiply,
        Subtract,
        Divide,
        Square,
        SquareRoot,
        Negate,
        Sine,
        Cosine,
    }

    public class MathAttributeModifier<TEnum> : AttributeModifier<TEnum> where TEnum : Enum
    {
        public TEnum Type;
        public MathOperation Operation;
        public float Operand;
        public float OriginalDuration;

        public MathAttributeModifier(
            TEnum type,
            float duration,
            MathOperation operation,
            float operand = 0f)
            : base(duration)
        {
            Type = type;
            Operation = operation;
            Operand = operand;
            OriginalDuration = duration;
        }

        public override void Handle(object sender, AttributeQuery<TEnum> query)
        {
            if (EqualityComparer<TEnum>.Default.Equals(query.Type, Type))
                query.Value = ApplyOperation(query.Value);
        }

        private float ApplyOperation(float x)
        {
            return Operation switch
            {
                MathOperation.Override => Operand,
                MathOperation.Add => x + Operand,
                MathOperation.Subtract => x - Operand,
                MathOperation.Multiply => x * Operand,
                MathOperation.Divide => x / Operand,
                MathOperation.Square => x * x,
                MathOperation.SquareRoot => MathF.Sqrt(x),
                MathOperation.Negate => -x,
                MathOperation.Sine => MathF.Sin(x),
                MathOperation.Cosine => MathF.Cos(x),
                _ => x
            };
        }

        public override AttributeModifier<TEnum> Clone()
        {
            var clone = new MathAttributeModifier<TEnum>(Type, OriginalDuration, Operation, Operand);

            if (Timer != null)
            {
                var remaining = Timer.Time;
                clone.Timer?.Reset(remaining);
            }

            return clone;
        }
    }
}