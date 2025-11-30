using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TooltipSystem
{
    public class Tooltip : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _characterWrapLimit;
        [SerializeField] private Vector2 _padding;

        [Header("Instances")]
        [SerializeField] private RectTransform _canvasRect;
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _contentText;
        [SerializeField] private LayoutElement _layoutElement;

        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            Vector2 newAnchorPos = Input.mousePosition / _canvasRect.localScale.x;

            newAnchorPos = new Vector2(Mathf.Clamp(newAnchorPos.x, _padding.x, _canvasRect.rect.width - _rect.rect.width - _padding.x), Mathf.Clamp(newAnchorPos.y, _padding.y, _canvasRect.rect.height - _rect.rect.height - _padding.y));
            newAnchorPos.y += _rect.rect.height;

            _rect.anchoredPosition = newAnchorPos;
        }

        public void SetText(string content, string header = "")
        {
            if (string.IsNullOrEmpty(header))
                _headerText.gameObject.SetActive(false);
            else
            {
                _headerText.gameObject.SetActive(true);
                _headerText.SetText(header);
            }

            _contentText.SetText(content);

            int headerLenght = _headerText.text.Length;
            int contentLenght = _contentText.text.Length;

            _layoutElement.enabled = (headerLenght > _characterWrapLimit || contentLenght > _characterWrapLimit) ? true : false;
        }
    }
}
