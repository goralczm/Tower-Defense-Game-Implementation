using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace UI
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
        [Header("Settings")]
        public UIAnimationType animationType;
        public Ease easeType;
        public float duration;
        public float delay;
        public int loopsCount;
        public bool startReversed;
        public bool canInterupt = false;

        [Header("Start & Final")]
        public bool setStartOnSetup;
        public bool setFinalOnSetup;
        public Vector3 startValue;
        public Vector3 finalValue;
        public bool useStartAsDifference;
        public bool useFinalAsDifference;

        [Header("Events")]
        public UnityEvent onShowCompleteCallback;
        public UnityEvent onHideCompleteCallback;

        private bool _hasSetup;
        private bool _isReversed;
        private RectTransform _rect;
        private CanvasGroup _canvasGroup;

        private bool _isRunning;

        public bool IsRunning => _isRunning;

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
                    if (setStartOnSetup)
                        startValue = _rect.anchoredPosition;
                    if (setFinalOnSetup)
                        finalValue = _rect.anchoredPosition;

                    if (startReversed)
                        _rect.anchoredPosition = finalValue;
                    else
                        _rect.anchoredPosition = startValue;
                    break;
                case UIAnimationType.Scale:
                    if (setStartOnSetup)
                        startValue = _rect.localScale;
                    if (setFinalOnSetup)
                        finalValue = _rect.localScale;

                    if (startReversed)
                        _rect.localScale = finalValue;
                    else
                        _rect.localScale = startValue;
                    break;
                case UIAnimationType.Fade:
                    if (!TryGetComponent(out _canvasGroup))
                        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                    if (startReversed)
                        _canvasGroup.alpha = finalValue.x;
                    else
                        _canvasGroup.alpha = startValue.x;
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
            if (IsRunning)
            {
                if (!canInterupt)
                    return;

                Kill();
            }

            _isReversed = false;
            HandleTween();
        }

        /// <summary>
        /// Plays the reversed tween.
        /// </summary>
        public void Hide()
        {
            if (IsRunning)
            {
                if (!canInterupt)
                    return;

                Kill();
            }

            _isReversed = true;
            HandleTween();
        }

        /// <summary>
        /// Toggles between playing normal and reversed tween.
        /// </summary>
        public void Toggle()
        {
            if (IsRunning)
            {
                if (!canInterupt)
                    return;

                Kill();
            }

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

            _isRunning = true;

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
            _isRunning = false;
        }

        /// <summary>
        /// Triggers the <see cref="onHideCompleteCallback"/> callback.
        /// </summary>
        private void OnHideComplete()
        {
            onHideCompleteCallback?.Invoke();
            _isRunning = false;
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
            Vector2 targetPos;

            if (!useFinalAsDifference)
            {
                if (!useStartAsDifference)
                    targetPos = _isReversed ? startValue : finalValue;
                else
                    targetPos = _isReversed ? startValue : finalValue + startValue;
            }
            else
                targetPos = _isReversed ? startValue : startValue + finalValue;

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
            Vector2 targetScale;

            if (!useFinalAsDifference)
                targetScale = _isReversed ? startValue : finalValue;
            else
                targetScale = _isReversed ? startValue : startValue + finalValue;

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

        public void Kill()
        {
            transform.DOKill();
        }
    }
}
