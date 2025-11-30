using GameSettings.Inputs.Basic;
using System.Collections.Generic;
using UnityEngine;

namespace GameSettings.Inputs.Extended
{
    public class FrameRateInput : DropdownSetting
    {
        private int[] _frameRates = new int[] { -1, 144, 120, 75, 60, 30 };

        public override void Setup()
        {
            List<string> options = new List<string>();

            for (int i = 0; i < _frameRates.Length; i++)
            {
                if (_frameRates[i] == -1)
                    options.Add("Unlimited");
                else
                    options.Add($"{_frameRates[i]} FPS");
            }

            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
        }

        public void SetFrameRate(int frameRatesIndex)
        {
            Application.targetFrameRate = _frameRates[frameRatesIndex];

            OnValueChanged();
        }
    }
}
