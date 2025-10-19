using UnityEngine;

namespace Utilities.CameraMovement
{
    [RequireComponent(typeof(SmoothDragCameraMovement), typeof(CameraScroll))]
    public class CameraDragForceConsideringScroll : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _minDragForce;
        [SerializeField] private float _maxDragForce;

        private SmoothDragCameraMovement _movement;
        private CameraScroll _scroll;
        private Camera _cam;

        private void Awake()
        {
            _movement = GetComponent<SmoothDragCameraMovement>();
            _scroll = GetComponent<CameraScroll>();
            _cam = _scroll.GetComponent<Camera>();
        }

        private void Update()
        {
            float minZoom = _scroll.GetMinZoom();
            float maxZoom = _scroll.GetMaxZoom();

            float zoomPercent = (_cam.orthographicSize - minZoom) / (maxZoom - minZoom);

            float targetDragForce = Mathf.Lerp(_minDragForce, _maxDragForce, zoomPercent);
            _movement.SetDragForce(targetDragForce);
        }
    }
}
