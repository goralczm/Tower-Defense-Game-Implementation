using UnityEngine;

namespace Utilities
{
    public static class GizmosHelpers
    {
        public static void DrawGizmosArrow(Vector3 start, Vector3 end, Color color, float headLength = 0.2f, float headAngle = 20f)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);

            Vector3 direction = (end - start).normalized;
            Vector3 right = Quaternion.Euler(0, 0, headAngle) * -direction;
            Vector3 left = Quaternion.Euler(0, 0, -headAngle) * -direction;

            Gizmos.DrawLine(end, end + right * headLength);
            Gizmos.DrawLine(end, end + left * headLength);
        }
    }
}
