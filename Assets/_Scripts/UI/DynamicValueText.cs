using TMPro;
using UnityEngine;

namespace Utilities.Utilities.UI.Texts
{
    /// <summary>
    /// Dynamically and gradually updates text based on the taget value.
    /// </summary>
    public class DynamicValueText : MonoBehaviour
    {
        [SerializeField] private int _value;

        [Header("Settings")]
        [SerializeField] private float _changeIntervals = .03f;

        [Header("Instances")]
        [SerializeField] private TextMeshProUGUI _text;

        private int _currentVisualValue;
        private float _lastTimeTick;

        private void Update()
        {
            if (_text == null)
                return;

            if (_currentVisualValue == _value)
                return;

            if (Time.time - _lastTimeTick >= _changeIntervals)
                Tick();
        }

        /// <summary>
        /// Calculates currently displayed value and sets the text accordingly.
        /// </summary>
        private void Tick()
        {
            CalculateValue();
            _text.SetText(_currentVisualValue.ToString());
            _lastTimeTick = Time.time;
        }

        /// <summary>
        /// Gradually updates the displayed value to match the target value.
        /// </summary>
        private void CalculateValue()
        {
            int diff = Mathf.Abs(_value - _currentVisualValue);

            int decimalPlaces = -1;
            while (diff > 0)
            {
                diff /= 10;
                decimalPlaces++;
            }

            if (_value > _currentVisualValue)
                _currentVisualValue += (int)Mathf.Pow(10, decimalPlaces);
            else
                _currentVisualValue -= (int)Mathf.Pow(10, decimalPlaces);
        }
        
        /// <summary>
        /// Sets the target value to be displayed.
        /// </summary>
        /// <param name="value">The target value.</param>
        public void SetValue(float value)
        {
            _value = (int)value;
        }
    }
}
