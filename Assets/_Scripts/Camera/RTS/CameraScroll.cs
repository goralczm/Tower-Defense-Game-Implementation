using Core;
using UnityEngine;

namespace Utilities.CameraMovement
{
    /// <summary>
    /// Handles the scrolling
    /// </summary>
    public class CameraScroll : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(1f, 10f)] private float _strength = 10f;
        [SerializeField, Range(0f, 1f)] private float _smootheness = .1f;
        [SerializeField] private float _minZoom = 2f, _maxZoom = 10f;

        [Header("Accessibility")]
        [SerializeField] private bool _reverseScroll;

        private Camera _cam;
        private float _targetCamSize;

        public float GetMinZoom() => _minZoom;

        public float GetMaxZoom() => _maxZoom;

        public void SetReverseScroll(bool state)
        {
            _reverseScroll = state;
        }

        private void Awake()
        {
            _cam = Camera.main;
            _targetCamSize = _cam.orthographicSize;
        }

        private void Update()
        {
            _targetCamSize = _targetCamSize - MouseInput.ScrollWheel * _strength * (_reverseScroll ? -1f : 1f);
            _targetCamSize = Mathf.Clamp(_targetCamSize, _minZoom, _maxZoom);

            if (_smootheness == 0)
                _cam.orthographicSize = _targetCamSize;
            else
                _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _targetCamSize, Time.deltaTime * (1 / _smootheness));
        }
    }
}
