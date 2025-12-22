using Core.Systems;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private string _soundName;
    [SerializeField] private bool _playOnEnable = true;
    [SerializeField] private bool _overrideIsMusic;

    private void OnEnable()
    {
        if (_playOnEnable)
            Play();
    }

    public void Play()
    {
        string groupName = !_overrideIsMusic ? "Sounds" : "Music";

        GlobalSystems.Instance.AudioSystem.PlaySoundFromGroup(groupName, _soundName);
    }
}
