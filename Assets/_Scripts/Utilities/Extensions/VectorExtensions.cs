using UnityEngine;

namespace Utilities.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 Add(this Vector2 v1, float x = 0, float y = 0)
        {
            return new Vector2(v1.x + x, v1.y + y);
        }

        public static Vector3 Add(this Vector3 v1, float x = 0, float y = 0, float z = 0)
        {
            return new Vector3(v1.x + x, v1.y + y, v1.z + z);
        }

        public static Vector2 Add(this Vector2 v1, Vector3 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x.HasValue ? v.x + x.Value : v.x, y.HasValue ? v.y + y.Value : v.y, z.HasValue ? v.z + z.Value : v.z);
        }

        public static Vector3Int ToVector3Int(this Vector2Int v)
        {
            return new Vector3Int(v.x, v.y, 0);
        }
    }
}
