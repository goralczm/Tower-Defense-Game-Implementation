using UnityEngine;

namespace Core
{
    public static class MouseInput
    {
        private static Camera _camera;

        public static Vector2 GetMousePosition() => Input.mousePosition;
        public static Vector2 GetMouseWorldPosition()
        {
            if (_camera == null)
                _camera = Camera.main;

            return _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        public static float ScrollWheel => UnityEngine.Input.GetAxis("Mouse ScrollWheel");
    }
}
