using System;
using System.Collections.Generic;

namespace Attributes
{
    public class AttributesMediator
    {
        private readonly LinkedList<AttributeModifier> _modifiers = new();

        public event EventHandler<AttributeQuery> Queries;

        public void PerformQuery(object sender, AttributeQuery query) => Queries?.Invoke(sender, query);

        public void AddModifier(AttributeModifier modifier)
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
    public class AttributeQuery
    {
        public AttributeType Type;
        public float Value;

        public AttributeQuery(AttributeType type, float value)
        {
            Type = type;
            Value = value;
        }
    }
}