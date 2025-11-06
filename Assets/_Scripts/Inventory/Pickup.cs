using UnityEngine;

namespace Inventory
{
    public class Pickup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _rend;

        private IItem _item;

        public IItem Item => _item;

        public void Setup(IItem item)
        {
            _item = item;
            _rend.sprite = _item.Icon;
            _rend.color = _item.Color;
        }
    }
}
