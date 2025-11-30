using UnityEngine;
using TMPro;

namespace Utilities.Utilities.UI.Texts
{
    /// <summary>
    /// Updates the text with target string, float or int value.
    /// </summary>
    public class ValueText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        /// <summary>
        /// Sets the text to be the target string value.
        /// </summary>
        /// <param name="value">The target string value.</param>
        public virtual void SetStringValue(string value)
        {
            _text.SetText(value);
        }

        /// <summary>
        /// Sets the text to be the target float value.
        /// </summary>
        /// <param name="value">The target float value.</param>
        /// <param name="decimalPlaces">The decimal places. (optional)</param>
        public virtual void SetFloatValue(float value, int decimalPlaces = 5)
        {
            _text.SetText(value.ToString($"F{decimalPlaces}"));
        }

        /// <summary>
        /// Sets the text to be the target int value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetIntValue(float value)
        {
            _text.SetText(((int)value).ToString());
        }
    }
}
