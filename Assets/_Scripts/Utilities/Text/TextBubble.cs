using ObjectPooling;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Gradle;
using UnityEngine;

namespace Utilities.Text
{
    public class TextBubble
    {
        private static Dictionary<GameObject, TextMeshPro> _cachedTexts = new Dictionary<GameObject, TextMeshPro>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearCache() => _cachedTexts = new();

        public static void CreateTextBubble(string text, Vector2 position, Color? color = null)
        {
            GameObject bubble = PoolManager.Instance.SpawnFromPool("Text Bubble", position, Quaternion.identity);

            SetupTextBubble(bubble, text, position, color);
        }

        public static void CreateSwirlyTextBubble(string text, Vector2 position, Color? color = null)
        {
            GameObject bubble = PoolManager.Instance.SpawnFromPool("Swirly Text Bubble", position, Quaternion.identity);

            SetupTextBubble(bubble, text, position, color);
        }

        private static void SetupTextBubble(GameObject bubble, string text, Vector2 position, Color? color = null)
        {
            if (!_cachedTexts.TryGetValue(bubble, out TextMeshPro textMeshPro))
            {
                textMeshPro = bubble.GetComponent<TextMeshPro>();
                _cachedTexts.Add(bubble, textMeshPro);
            }

            textMeshPro.SetText(text);
            if (color.HasValue)
                textMeshPro.color = color.Value;
        }
    }
}
