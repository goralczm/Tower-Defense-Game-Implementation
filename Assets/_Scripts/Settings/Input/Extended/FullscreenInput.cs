using GameSettings.Inputs.Basic;
using UnityEngine;

namespace GameSettings.Inputs.Extended
{
    public class FullscreenInput : BoolSetting
    {
        public void ToggleFullscreen(bool state)
        {
            Screen.fullScreen = state;

            OnValueChanged();
        }
    }
}
