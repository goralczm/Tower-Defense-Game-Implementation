using UnityEngine;

namespace Utilities.CameraMovement
{
    /// <summary>
    /// Handles the camera movement following the target considering the position of mouse.
    /// </summary>
    public class CameraMouseTargetFollow : MonoBehaviour
    {
        [Header("Mouse Settings")]
        [SerializeField, Range(.5f, 2f)] private float _smoothness = 2f;
        [Tooltip("How much the camera offsets the target position with mouse position")]
        [SerializeField, Range(0f, .5f)] private float _mouseFavoritism = .15f;

        [Header("Movement Settings")]
        [Tooltip("Controls how rapidly the camera moves if the mouse moves far away in short time.")]
        [SerializeField, Range(1f, 15f)] private float _maxAcceleration = 10f;
        [SerializeField] private Vector2 _offset;

        [Header("Instances")]
        [SerializeField] private UnityEngine.Transform _target;

        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            float distanceBtwTarget = Vector2.Distance(transform.position, _target.position);

            Vector3 targetPos = Vector2.Lerp(_target.position, _cam.ScreenToWorldPoint(Input.mousePosition), _mouseFavoritism);
            targetPos = (Vector2)targetPos + _offset;
            targetPos.z = -10;

            float changeSpeed = (1f / _smoothness) * Mathf.Clamp(distanceBtwTarget, 1f, _maxAcceleration);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * changeSpeed);
        }
    }
}
