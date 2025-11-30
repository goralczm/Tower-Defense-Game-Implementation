using UnityEngine;
using UnityEngine.Audio;
using GameSettings.Inputs.Basic;

namespace GameSettings.Inputs.Extended
{
    public class VolumeInput : SliderSetting
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private string _exposedVolumeName;

        public override void PreviousOption()
        {
            _slider.value -= 5;
        }

        public override void NextOption()
        {
            _slider.value += 5;
        }

        public void SetVolume(float value)
        {
            if (value > _slider.minValue)
                _mixer.SetFloat(_exposedVolumeName, value);
            else
                _mixer.SetFloat(_exposedVolumeName, -80f);

            OnValueChanged();
        }
    }
}
