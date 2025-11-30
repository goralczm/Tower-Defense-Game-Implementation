using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameSettings.Inputs.Basic
{
    public class ArrowsSetting : SettingsInput
    {
        [Header("Settings")]
        [SerializeField] private List<string> _options = new List<string>() { "Option A", "Option B", "Option C" };
        [SerializeField] private int _deaultValueIndex;
        [SerializeField] private bool _loopOptions;

        [Header("Instances")]
        [SerializeField] private TextMeshProUGUI _displayedText;

        public Action<string> OnOptionsChanged;

        private int _selectedOption = 0;

        public override void Setup()
        {
            base.Setup();

            OnOptionsChanged += UpdateDisplayedText;
        }

        public override void NextOption()
        {
            SetOption(_selectedOption + 1);
        }

        public override void PreviousOption()
        {
            SetOption(_selectedOption - 1);
        }

        public override void ResetToDefault()
        {
            SetOption(_deaultValueIndex);
        }

        public override void Load(object data)
        {
            SetOption(Convert.ToInt32(data));
        }

        public override object Save()
        {
            return _selectedOption;
        }

        public void SetOption(int index)
        {
            if (!_loopOptions)
                index = Mathf.Clamp(index, 0, _options.Count - 1);
            else
            {
                if (index > _options.Count - 1)
                    index = 0;

                if (index < 0)
                    index = _options.Count - 1;
            }
            _selectedOption = index;

            OnOptionsChanged?.Invoke(_options[index]);
        }

        public void UpdateDisplayedText(string text)
        {
            _displayedText.SetText(text);
        }
    }
}
