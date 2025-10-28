using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Utilities.UI
{
    /// <summary>
    /// Tweening animation type.
    /// </summary>
    public enum UIAnimationType
    {
        Move,
        Scale,
        Fade
    }

    /// <summary>
    /// Provides a set of tools to dynamically tween the UI elements using DoTween library.
    /// </summary>
    public class UITweener : MonoBehaviour
    {
        [SerializeField] private UIAnimationType animationType;
        [SerializeField] private Ease easeType;
        [SerializeField] private float duration;
        [SerializeField] private float delay;

        [SerializeField] private int loopsCount;
        [SerializeField] private bool startReversed;

        [SerializeField] private Vector3 startValue;
        [SerializeField] private Vector3 finalValue;

        [SerializeField] private UnityEvent onShowCompleteCallback;
        [SerializeField] private UnityEvent onHideCompleteCallback;

        private bool _hasSetup;
        private bool _isReversed;
        private RectTransform _rect;
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// Caches internal references and sets the default values of element based on <see cref="_hasSetup"/>.
        /// </summary>
        private void Setup()
        {
            if (_hasSetup)
                return;

            _rect = GetComponent<RectTransform>();

            switch (animationType)
            {
                case UIAnimationType.Move:
                    _rect.anchoredPosition = startValue;
                    if (startReversed)
                        _rect.anchoredPosition = finalValue;
                    break;
                case UIAnimationType.Scale:
                    _rect.localScale = startValue;
                    if (startReversed)
                        _rect.localScale = finalValue;
                    break;
                case UIAnimationType.Fade:
                    _canvasGroup = GetComponent<CanvasGroup>();
                    if (_canvasGroup == null)
                        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                    _canvasGroup.alpha = startValue.x;
                    if (startReversed)
                        _canvasGroup.alpha = finalValue.x;
                    break;
            }

            if (startReversed)
                _isReversed = true;

            _hasSetup = true;
        }

        /// <summary>
        /// Plays the tween.
        /// </summary>
        public void Show()
        {
            _isReversed = false;
            HandleTween();
        }

        /// <summary>
        /// Plays the reversed tween.
        /// </summary>
        public void Hide()
        {
            _isReversed = true;
            HandleTween();
        }

        /// <summary>
        /// Toggles between playing normal and reversed tween.
        /// </summary>
        public void Toggle()
        {
            _isReversed = !_isReversed;
            HandleTween();
        }

        /// <summary>
        /// Handles the tweening animation.
        /// </summary>
        private void HandleTween()
        {
            Setup();

            if (!_isReversed)
                gameObject.SetActive(true);

            switch (animationType)
            {
                case UIAnimationType.Fade:
                    Fade();
                    break;
                case UIAnimationType.Move:
                    Move();
                    break;
                case UIAnimationType.Scale:
                    Scale();
                    break;
            }
        }

        /// <summary>
        /// Triggers the <see cref="onShowCompleteCallback"/> callback.
        /// </summary>
        private void OnShowComplete()
        {
            onShowCompleteCallback?.Invoke();
        }

        /// <summary>
        /// Triggers the <see cref="onHideCompleteCallback"/> callback.
        /// </summary>
        private void OnHideComplete()
        {
            onHideCompleteCallback?.Invoke();
        }
        
        /// <summary>
        /// Tweens the UI element with fade animation.
        /// </summary>
        private void Fade()
        {
            float targetFade = _isReversed ? startValue.x : finalValue.x;

            var tween = _canvasGroup.DOFade(targetFade, duration)
                                    .SetDelay(delay)
                                    .SetEase(easeType)
                                    .SetUpdate(true);

            if (loopsCount != 0)
                tween.SetLoops(loopsCount);

            if (_isReversed)
                tween.onComplete += OnHideComplete;
            else
                tween.onComplete += OnShowComplete;
        }

        /// <summary>
        /// Tweens the UI element with move animation.
        /// </summary>
        private void Move()
        {
            Vector2 targetPos = _isReversed ? startValue : finalValue;

            var tween = _rect.DOAnchorPos(targetPos, duration)
                             .SetDelay(delay)
                             .SetEase(easeType)
                             .SetUpdate(true);

            if (loopsCount != 0)
                tween.SetLoops(loopsCount);

            if (_isReversed)
                tween.onComplete += OnHideComplete;
            else
                tween.onComplete += OnShowComplete;
        }

        /// <summary>
        /// Tweens the UI element with scale animation.
        /// </summary>
        private void Scale()
        {
            Vector2 targetScale = _isReversed ? startValue : finalValue;

            var tween = transform.DOScale(targetScale, duration)
                                 .SetDelay(delay)
                                 .SetEase(easeType)
                                 .SetUpdate(true);

            if (loopsCount != 0)
                tween.SetLoops(loopsCount);

            if (_isReversed)
                tween.onComplete += OnHideComplete;
            else
                tween.onComplete += OnShowComplete;
        }
    }
}
