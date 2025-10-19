using UnityEngine;

namespace Utilities.CameraMovement
{
    /// <summary>
    /// Handles the camera movement following the target.
    /// Additionally allows to customize the movement with heads up, offset and gravity 
    /// </summary>
    public class CameraFollowTarget : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _followSpeed = 2f;
        [SerializeField] private Vector2 _offset;

        [Header("Axis Heads Up Settings")]
        [SerializeField] private float _horizontalHeadsUpStrength;
        [SerializeField] private float _verticalHeadsUpStrength;

        [Header("Gravity Heads Up Settings")]
        [SerializeField, Range(0, 2f)] private float _risingVelocityHeadsupStrength;
        [SerializeField, Range(0, 2f)] private float _fallingVelocityHeadsupStrength;

        [Header("Instances")]
        [SerializeField] private UnityEngine.Transform _target;

        private Rigidbody2D _targetRb;

        private const string HORIZONTAL_AXIS = "Horizontal";
        private const string VERTICAL_AXIS = "Vertical";

        private void Start()
        {
            if (_target == null)
                return;

            _targetRb = _target.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (_target == null)
                return;

            Vector2 inputHeadsupOffset = GetInputHeadsupOffset();
            Vector2 velocityHeadsupOffset = GetVelocityHeadsupOffset();
            Vector2 targetPos = GetTargetPos(inputHeadsupOffset, velocityHeadsupOffset);

            LerpToPosition(targetPos);
        }

        /// <summary>
        /// Offsets the input with a heads up settings.
        /// </summary>
        /// <returns>The offseted input.</returns>
        private Vector2 GetInputHeadsupOffset()
        {
            Vector2 input = GetInput();

            return input * new Vector2(_horizontalHeadsUpStrength, _verticalHeadsUpStrength);
        }

        /// <summary>
        /// Returns the input based on the <see cref="Input"/> axis.
        /// </summary>
        /// <returns>The input.</returns>
        private Vector2 GetInput()
        {
            float horizontalAxis = Input.GetAxis(HORIZONTAL_AXIS);
            float verticalAxis = Input.GetAxis(VERTICAL_AXIS);

            return new Vector2(horizontalAxis, verticalAxis);
        }

        /// <summary>
        /// Checks for the rigidbody target and applies the gravity heads up if exists.
        /// </summary>
        /// <returns>The gravity offseted input.</returns>
        private Vector2 GetVelocityHeadsupOffset()
        {
            if (_targetRb == null)
                return Vector2.zero;

            Vector2 velocityOffset = new Vector2(0, _targetRb.linearVelocity.y);

            if (_targetRb.linearVelocity.y < 0)
                velocityOffset *= _fallingVelocityHeadsupStrength;

            if (_targetRb.linearVelocity.y > 0)
                velocityOffset *= _risingVelocityHeadsupStrength;

            return velocityOffset;
        }

        /// <summary>
        /// Returns the combined target position with heads up settings.
        /// </summary>
        /// <param name="axisHeadsupOffset">Axis heads up offset.</param>
        /// <param name="gravityHeadsupOffset">Gravity heads up offset.</param>
        /// <returns>The combined position.</returns>
        private Vector2 GetTargetPos(Vector2 axisHeadsupOffset, Vector2 gravityHeadsupOffset)
        {
            return (Vector2)_target.position + _offset + axisHeadsupOffset + gravityHeadsupOffset;
        }

        /// <summary>
        /// Calculates and updates the next smooth position between current position and target position.
        /// </summary>
        /// <param name="targetPos">The target position.</param>
        private void LerpToPosition(Vector2 targetPos)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, targetPos, _followSpeed * Time.deltaTime);
            newPos.z = -10;

            transform.position = newPos;
        }
    }
}
