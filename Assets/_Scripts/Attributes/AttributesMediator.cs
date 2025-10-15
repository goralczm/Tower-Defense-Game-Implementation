using System;
using System.Collections.Generic;

namespace Attributes
{
    public class AttributesMediator<TEnum> where TEnum : Enum
    {
        private readonly LinkedList<AttributeModifier<TEnum>> _modifiers = new();

        public event EventHandler<AttributeQuery<TEnum>> Queries;

        public void PerformQuery(object sender, AttributeQuery<TEnum> query) => Queries?.Invoke(sender, query);

        public void AddModifier(AttributeModifier<TEnum> modifier)
        {
            _modifiers.AddLast(modifier);
            Queries += modifier.Handle;

            modifier.OnDispose += _ =>
            {
                _modifiers.Remove(modifier);
                Queries -= modifier.Handle;
            };
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
    }

    [System.Serializable]
    public class AttributeQuery<TEnum> where TEnum : Enum
    {
        public TEnum Type;
        public float Value;

        public AttributeQuery(TEnum type, float value)
        {
            Type = type;
            Value = value;
        }
    }
}