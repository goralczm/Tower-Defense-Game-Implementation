using UnityEngine;

namespace Utilities
{
    public static class ColorExtensions
    {
        public static Color SetOpacity(this Color c, float opacity)
        {
            c.a = Mathf.Clamp(opacity, 0, 1);
            return c;
        }
    }
}
