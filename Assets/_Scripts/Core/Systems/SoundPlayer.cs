using Core.Systems;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private string _soundName;
    [SerializeField] private bool _playOnEnable = true;

    private void OnEnable()
    {
        if (_playOnEnable)
            Play();
    }

    public void Play()
    {
        GlobalSystems.Instance.AudioSystem.PlaySoundFromGroup("Sounds", _soundName);
    }
}
