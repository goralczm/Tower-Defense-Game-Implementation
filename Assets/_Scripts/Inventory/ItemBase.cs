using UnityEngine;

namespace Inventory
{
    public abstract class ItemBase : ScriptableObject, IItem
    {
        public abstract int Id { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract Sprite Icon { get; }

        public abstract Color Color { get; }
    }
}
