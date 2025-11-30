using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameSettings.Inputs.Basic;

namespace GameSettings.Inputs.Extended
{
    public class ResolutionInput : DropdownSetting
    {
        [SerializeField] private int _minWidth, _minHeight;

        private List<Resolution> _resolutions = new List<Resolution>();

        public override void RevertLast()
        {
            base.RevertLast();
            _settings.ConfirmSetting();
        }

        public override void Setup()
        {
            _resolutions = Screen.resolutions.ToList();

            List<string> options = new List<string>();
            for (int i = _resolutions.Count - 1; i >= 0; i--)
            {
                if (_resolutions[i].width < _minWidth)
                {
                    _resolutions.RemoveAt(i);
                    continue;
                }

                if (_resolutions[i].height < _minHeight)
                {
                    _resolutions.RemoveAt(i);
                    continue;
                }

                string option = _resolutions[i].width + " x " + _resolutions[i].height;

                if (options.Contains(option))
                {
                    _resolutions.RemoveAt(i);
                    continue;
                }

                options.Add(option);
            }

            _resolutions.Reverse();

            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution res = _resolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);

            OnValueChanged();
        }
    }
}
