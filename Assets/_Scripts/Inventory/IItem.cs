using UnityEngine;

namespace Inventory
{
    public interface IItem
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public Sprite Icon { get; }
        public Color Color { get; }
    }
}
