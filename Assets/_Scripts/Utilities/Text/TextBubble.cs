using ObjectPooling;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Utilities.Text
{
    public class TextBubble
    {
        private Dictionary<GameObject, TextMeshPro> _cachedTexts = new Dictionary<GameObject, TextMeshPro>();

        public TextBubble(string text, Vector2 position, Color? color = null)
        {
            GameObject bubble = PoolManager.Instance.SpawnFromPool("Text Bubble", position, Quaternion.identity);

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
