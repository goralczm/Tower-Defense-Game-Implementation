using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameSettings.Inputs.Basic
{
    public class SliderSetting : SettingsInput
    {
        [Header("Settings")]
        [SerializeField] private float _defaultValue;

        [Header("Instances")]
        [SerializeField] protected Slider _slider;

        public override void PreviousOption()
        {
            _slider.value -= 1;
        }

        public override void NextOption()
        {
            _slider.value += 1;
        }

        public override void RevertLast()
        {
            base.RevertLast();
            _slider.value = (float)_valueHistory.Pop();
        }

        public override void ResetToDefault()
        {
            _slider.value = _defaultValue;
        }

        public override object Save()
        {
            return _slider.value;
        }

        public override void Load(object data)
        {
            _slider.value = Convert.ToSingle(data);
        }
    }
}
