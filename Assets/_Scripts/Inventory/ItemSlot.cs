using TooltipSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public class ItemSlot : TooltipTrigger, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Header("References")]
        [SerializeField] private Image _icon;

        private IItem _item;
        private Inventory _inventory;
        private Transform _originalParent;
        private Transform _draggedItemParent;
        private int _index;

        public override string Header => _item?.Name ?? "";
        public override string Content => _item?.Description ?? "";

        public void Init(Inventory inventory, Transform draggedItemParent)
        {
            _inventory = inventory;
            _draggedItemParent = draggedItemParent;
        }

        public void Setup(IItem item, int index)
        {
            _item = item;
            _index = index;

            _icon.enabled = _item != null;
            if (_item != null)
            {
                _icon.enabled = true;
                _icon.sprite = _item.Icon;
                _icon.color = _item.Color;
            }
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            ItemSlot draggedSlot = eventData.pointerDrag.GetComponentInParent<ItemSlot>();

            if (draggedSlot == this)
                return;

            int oldIndex = _index;
            IItem newItem = draggedSlot._inventory.Swap(_item, draggedSlot._index);
            _inventory.Swap(newItem, oldIndex);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent = _icon.transform.parent;
            _icon.transform.SetParent(_draggedItemParent, true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _icon.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _icon.transform.SetParent(_originalParent, true);
            _icon.transform.localPosition = Vector2.zero;
        }
    }
}
