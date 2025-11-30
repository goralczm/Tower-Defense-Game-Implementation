using System;
using System.Collections.Generic;

namespace Attributes
{
    [System.Serializable]
    public class AttributesMediator<TEnum> : ICloneable where TEnum : Enum
    {
        public LinkedList<AttributeModifier<TEnum>> Modifiers = new();

        public event EventHandler<AttributeQuery<TEnum>> Queries;
        public event Action OnAttributesChanged;

        public void PerformQuery(object sender, AttributeQuery<TEnum> query) => Queries?.Invoke(sender, query);

        public void ForceReconnectModifiers()
        {
            Queries = null;

            foreach (var modifier in Modifiers)
            {
                modifier.ForceTimerSetup();
                Queries += modifier.Handle;

                modifier.OnDispose += _ =>
                {
                    Modifiers.Remove(modifier);
                    Queries -= modifier.Handle;
                    OnAttributesChanged?.Invoke();
                };
            }
        }

        public void AddModifier(AttributeModifier<TEnum> modifier)
        {
            Modifiers.AddLast(modifier);
            Queries += modifier.Handle;

            modifier.OnDispose += _ =>
            {
                Modifiers.Remove(modifier);
                Queries -= modifier.Handle;
                OnAttributesChanged?.Invoke();
            };

            OnAttributesChanged?.Invoke();
        }

        public void Update(float deltaTime)
        {
            var node = Modifiers.First;
            while (node != null)
            {
                var modifier = node.Value;
                modifier.Update(deltaTime);
                node = node.Next;
            }

            node = Modifiers.First;
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

            foreach (var modifier in Modifiers)
            {
                var clonedModifier = modifier.Clone();
                clone.Modifiers.AddLast(clonedModifier);
                clone.Queries += clonedModifier.Handle;

                clonedModifier.OnDispose += _ =>
                {
                    clone.Modifiers.Remove(clonedModifier);
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