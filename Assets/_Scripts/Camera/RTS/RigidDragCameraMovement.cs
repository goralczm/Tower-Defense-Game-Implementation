using Core;
using UnityEngine;

namespace Utilities.CameraMovement
{
    /// <summary>
    /// Rigidly drags the camera based on the mouse input.
    /// </summary>
    public class RigidDragCameraMovement : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("0 - Left Mouse Button\n1 - Right Mouse Button\n2 - Middle Mouse Button")]
        [SerializeField] private int _dragMouseButton = 0;
        [SerializeField] private Bounds _boundry;

        private Vector2 _startDragPos;


        private void Update()
        {
            if (Input.GetMouseButtonDown(_dragMouseButton))
            {
                _startDragPos = MouseInput.GetMousePosition();
                return;
            }

            if (!Input.GetMouseButton(_dragMouseButton))
                return;

            Vector2 offset = MouseInput.GetMousePosition() - (Vector2)transform.position;
            Vector2 dir = _startDragPos - offset;
            if (_boundry != null)
                dir = _boundry.ClosestPoint(dir);
            Vector3 newPos = new Vector3(dir.x, dir.y, -10);

            transform.position = newPos;
        }

        private void OnValidate()
        {
            _dragMouseButton = Mathf.Clamp(_dragMouseButton, 0, 2);
        }
    }
}
