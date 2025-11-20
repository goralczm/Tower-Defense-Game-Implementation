using UnityEngine;
using UnityEngine.EventSystems;

namespace TooltipSystem
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string _header;
        [SerializeField, TextArea(7, 7)] private string _content;

        private const float DELAY = .5f;

        private float _delayTimer;
        private bool _isWaiting;

        public virtual string Header => _header;
        public virtual string Content => _content;

        public virtual void Update()
        {
            if (_isWaiting)
            {
                _delayTimer -= Time.deltaTime;
                if (_delayTimer <= 0)
                {
                    if (!string.IsNullOrEmpty(Content) || !string.IsNullOrEmpty(Header))
                    {
                        TooltipSystem.Show(Content, Header);
                        _isWaiting = false;
                    }
                }
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
        }

        public void OnMouseEnter()
        {
            ShowTooltip();
        }

        private void OnMouseExit()
        {
            HideTooltip();
        }

        private void ShowTooltip()
        {
            _delayTimer = DELAY;
            _isWaiting = true;
        }

        private void HideTooltip()
        {
            _isWaiting = false;
            TooltipSystem.Hide();
        }
    }
}
