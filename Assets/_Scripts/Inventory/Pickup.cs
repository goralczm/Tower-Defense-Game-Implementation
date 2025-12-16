using UnityEngine;

namespace Inventory
{
    public class Pickup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _hideTimer = 15f;

        [Header("References")]
        [SerializeField] private SpriteRenderer _rend;

        private IItem _item;
        private float _startTime;

        public IItem Item => _item;
        public float TimeLeft => Mathf.Clamp(_startTime + _hideTimer - Time.time, 0f, _hideTimer);
        public float HideTimer => _hideTimer;

        public void Setup(IItem item)
        {
            _item = item;
            _rend.sprite = _item.Icon;
            _rend.color = _item.Color;

            _startTime = Time.time;
            Invoke("HideSelf", _hideTimer);
        }

        private void HideSelf()
        {
            gameObject.SetActive(false);
        }
    }
}
