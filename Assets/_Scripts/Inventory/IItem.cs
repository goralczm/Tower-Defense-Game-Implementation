using UnityEngine;

namespace Inventory
{
    public interface IItem
    {
        public string Name { get; }
        public string Description { get; }
        public Sprite Icon { get; }
        public Color Color { get; }
    }
}
