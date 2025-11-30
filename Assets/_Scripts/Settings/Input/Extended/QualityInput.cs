using GameSettings.Inputs.Basic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameSettings.Input.Extended
{
    public class QualityInput : DropdownSetting
    {
        public override void Setup()
        {
            List<string> options = QualitySettings.names.ToList();

            options.Reverse();

            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
        }

        public void SetQualityLevel(int qualityLevel)
        {
            QualitySettings.SetQualityLevel(_dropdown.options.Count - 1 - qualityLevel);

            OnValueChanged();
        }
    }
}
