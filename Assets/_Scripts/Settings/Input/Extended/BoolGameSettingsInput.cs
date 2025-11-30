using GameSettings.Inputs.Basic;

namespace GameSettings.Inputs.Extended
{
    public class BoolGameSettingsInput : BoolSetting
    {
        public void ToggleSetting(bool state)
        {
            CustomGameSettings.SaveSetting(SettingName, state);

            OnValueChanged();
        }
    }
}
