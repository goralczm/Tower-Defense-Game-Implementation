using System;
using System.Collections.Generic;

namespace Attributes
{
    public class AttributesMediator<TEnum> : ICloneable where TEnum : Enum
    {
        private readonly LinkedList<AttributeModifier<TEnum>> _modifiers = new();

        public event EventHandler<AttributeQuery<TEnum>> Queries;

        public event Action OnAttributesChanged;

        public LinkedList<AttributeModifier<TEnum>> Modifiers => _modifiers;

        public void PerformQuery(object sender, AttributeQuery<TEnum> query) => Queries?.Invoke(sender, query);

        public void AddModifier(AttributeModifier<TEnum> modifier)
        {
            _modifiers.AddLast(modifier);
            Queries += modifier.Handle;

            modifier.OnDispose += _ =>
            {
                _modifiers.Remove(modifier);
                Queries -= modifier.Handle;
                OnAttributesChanged?.Invoke();
            };

            OnAttributesChanged?.Invoke();
        }

        public void Update(float deltaTime)
        {
            var node = _modifiers.First;
            while (node != null)
            {
                var modifier = node.Value;
                modifier.Update(deltaTime);
                node = node.Next;
            }

            node = _modifiers.First;
            while (node != null)
            {
                var nextNode = node.Next;

                if (node.Value.MarkedForRemoval)
                    node.Value.Dispose();

                node = nextNode;
            }
        }

        public AttributesMediator<TEnum> Clone()
        {
            var clone = new AttributesMediator<TEnum>();

            foreach (var modifier in _modifiers)
            {
                var clonedModifier = modifier.Clone();
                clone._modifiers.AddLast(clonedModifier);
                clone.Queries += clonedModifier.Handle;

                clonedModifier.OnDispose += _ =>
                {
                    clone._modifiers.Remove(clonedModifier);
                    clone.Queries -= clonedModifier.Handle;
                    clone.OnAttributesChanged?.Invoke();
                };
            }

            return clone;
        }

        object ICloneable.Clone() => Clone();
    }

    [System.Serializable]
    public class AttributeQuery<TEnum> : ICloneable where TEnum : Enum
    {
        public TEnum Type;
        public float Value;

        public AttributeQuery(TEnum type, float value)
        {
            Type = type;
            Value = value;
        }

        public static AttributeQuery<TEnum> operator +(AttributeQuery<TEnum> a, AttributeQuery<TEnum> b)
        {
            if (!a.Type.Equals(b.Type))
                throw new InvalidOperationException($"Cannot add AttributeQuery of different types ({a.Type} vs {b.Type})");

            return new AttributeQuery<TEnum>(a.Type, a.Value + b.Value);
        }

        public static AttributeQuery<TEnum> operator -(AttributeQuery<TEnum> a, AttributeQuery<TEnum> b)
        {
            if (!a.Type.Equals(b.Type))
                throw new InvalidOperationException($"Cannot subtract AttributeQuery of different types ({a.Type} vs {b.Type})");

            return new AttributeQuery<TEnum>(a.Type, a.Value - b.Value);
        }

        public AttributeQuery<TEnum> Clone()
        {
            return new AttributeQuery<TEnum>(Type, Value);
        }

        object ICloneable.Clone() => Clone();
    }
}