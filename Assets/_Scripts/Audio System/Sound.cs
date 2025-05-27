using UnityEngine;

namespace AudioSystem
{
    /// <summary>
    /// Represents a sound clip along with its properties such as volume, pitch, and looping behavior.
    /// </summary>
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = .2f;
        [Range(.1f, 3f)] public float pitch = 1;
        public bool loop;
        [HideInInspector] public AudioSource source;
    }
}
