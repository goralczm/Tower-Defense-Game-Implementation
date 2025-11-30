using UnityEngine;
using TMPro;
using System;

namespace GameSettings.Inputs.Basic
{
    public class DropdownSetting : SettingsInput
    {
        [Header("Settings")]
        [SerializeField] protected int _defaultValue;

        [Header("Instances")]
        [SerializeField] protected TMP_Dropdown _dropdown;

        public override void PreviousOption()
        {
            _dropdown.value = Mathf.Clamp(_dropdown.value - 1, 0, _dropdown.options.Count - 1);
        }

        public override void NextOption()
        {
            _dropdown.value = Mathf.Clamp(_dropdown.value + 1, 0, _dropdown.options.Count - 1);
        }

        public override void RevertLast()
        {
            base.RevertLast();
            _dropdown.value = (int)_valueHistory.Pop();
        }

        public override void ResetToDefault()
        {
            _dropdown.value = _defaultValue;
        }

        public override object Save()
        {
            return _dropdown.value;
        }

        public override void Load(object data)
        {
            _dropdown.value = Convert.ToInt32(data);
        }
    }
}
