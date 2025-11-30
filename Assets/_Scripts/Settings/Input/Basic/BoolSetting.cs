using UnityEngine;
using UnityEngine.UI;

namespace GameSettings.Inputs.Basic
{
    public class BoolSetting : SettingsInput
    {
        [Header("Settings")]
        [SerializeField] private bool _defaultValue;

        [Header("Instances")]
        [SerializeField] protected Toggle _toggle;

        public override void PreviousOption()
        {
            _toggle.isOn = !_toggle.isOn;
        }

        public override void NextOption()
        {
            _toggle.isOn = !_toggle.isOn;
        }

        public override void RevertLast()
        {
            base.RevertLast();
            _toggle.isOn = (bool)_valueHistory.Pop();
        }

        public override void ResetToDefault()
        {
            _toggle.isOn = _defaultValue;
        }

        public override object Save()
        {
            return _toggle.isOn;
        }

        public override void Load(object data)
        {
            _toggle.isOn = (bool)data;
        }
    }
}
