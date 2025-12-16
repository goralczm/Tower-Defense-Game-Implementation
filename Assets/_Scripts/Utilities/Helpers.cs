using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public static class Helpers
    {
        public static string LimitDecimalPoints(this float value, int decimals)
        {
            if (value % 1 == 0)
                return value.ToString();

            return value.ToString($"n{decimals}");
        }

        public static bool IsMouseOverUI()
        {
            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(0);

            return EventSystem.current.IsPointerOverGameObject();
        }

        public static bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        public static Quaternion RotateTowards(Vector2 origin, Vector2 target, float offset = 0f)
        {
            Vector3 dir = target - origin;
            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            return Quaternion.Euler(0, 0, rotZ + offset);
        }

        public static async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
        {
            while (!condition())
                await Task.Delay(checkIntervalMs);
        }

        public static string GetLevelName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public static void SaveScreenshot(string path, string screenshotName)
        {
            ScreenCapture.CaptureScreenshot($"{path}\\{screenshotName}.png");
        }
    }
}
