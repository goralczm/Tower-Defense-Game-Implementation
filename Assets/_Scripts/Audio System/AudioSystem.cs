using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    /// <summary>
    /// Manages the playing of sounds and music in the game.
    /// </summary>
    public class AudioSystem : MonoBehaviour
    {
        [Header("Sound Groups")]
        [SerializeField] private SoundGroup[] _soundGroups;

        private Dictionary<string, SoundGroup> _cachedSoundGroups = new Dictionary<string, SoundGroup>();

        protected void Awake()
        {
            //base.Awake();
            foreach (SoundGroup group in _soundGroups)
            {
                _cachedSoundGroups.Add(group.name, group);
                foreach (Sound sound in group.sounds)
                {
                    group.AddSoundToDicitonary(sound);
                    CreateSoundInstance(sound, group.mixerGroup);
                }
            }
        }

        /// <summary>
        /// Creates and caches a sound instance in the <see cref="GameObject"/>.
        /// </summary>
        /// <param name="sound">The sound to be played.</param>
        /// <param name="mixerGroup">The mixer group that the sound should be routed."/></param>
        private void CreateSoundInstance(Sound sound, AudioMixerGroup mixerGroup)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.outputAudioMixerGroup = mixerGroup;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }

        /// <summary>
        /// Retrieves the <see cref="SoundGroup"/> from the cached dictionary based on the provided group name.
        /// </summary>
        /// <param name="groupName">The name of the sound group.</param>
        /// <returns>The <see cref="SoundGroup"/> associated with the provided group name.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the requested sound group is not found in the cache.</exception>
        private SoundGroup FindSoundGroup(string groupName)
        {
            if (!_cachedSoundGroups.ContainsKey(groupName))
                throw new KeyNotFoundException($"Given '{groupName}' sound group could not be found!");

            return _cachedSoundGroups[groupName];
        }

        /// <summary>
        /// Retrieves the <see cref="Sound"/> from the cached dictionary based on the provided group name and sound name.
        /// </summary>
        /// <param name="groupName">The name of the sound group.</param>
        /// <param name="soundName">The name of the sound.</param>
        /// <returns>The <see cref="Sound"/> associated with the provided group name.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the requested sound group or sound name is not found in the cache.</exception>
        private Sound FindSound(string groupName, string soundName)
        {
            SoundGroup soundGroup = FindSoundGroup(groupName);
            Sound sound = soundGroup.GetSound(soundName);

            return sound;
        }

        /// <summary>
        /// Plays the sound based on the provided <see cref="SoundGroup"/> name and sound name.
        /// </summary>
        /// <param name="groupName">The name of the sound group.</param>
        /// <param name="soundName">The name of the sound.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the requested sound group or sound name is not found in the cache.</exception>
        public void PlaySoundFromGroup(string groupName, string soundName)
        {
            Sound sound = FindSound(groupName, soundName);

            sound.source.Play();
        }

        /// <summary>
        /// Plays the sound based on the provided <see cref="SoundGroup"/> name and sound name if the sound is not yet playing.
        /// </summary>
        /// <param name="groupName">The name of the sound group.</param>
        /// <param name="soundName">The name of the sound.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the requested sound group or sound name is not found in the cache.</exception>
        public void PlaySoundFromGroupIfNotPlaying(string groupName, string soundName)
        {
            Sound sound = FindSound(groupName, soundName);

            if (sound.source.isPlaying)
                return;

            sound.source.Play();
        }

        /// <summary>
        /// Plays random sound from the provided <see cref="SoundGroup"/> name.
        /// </summary>
        /// <param name="groupName">The name of the sound group.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the requested sound group is not found in the cache.</exception>
        public void PlayRandomSoundFromGroup(string groupName)
        {
            SoundGroup soundGroup = FindSoundGroup(groupName);

            int randomSoundIndex = Random.Range(0, soundGroup.sounds.Length);
            PlaySoundFromGroup(groupName, soundGroup.sounds[randomSoundIndex].name);
        }

        /// <summary>
        /// Stops the sound from playing.
        /// </summary>
        /// <param name="groupName">The name of the sound group.</param>
        /// <param name="soundName">The name of the sound.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the requested sound group or sound name is not found in the cache.</exception>
        public void StopSound(string groupName, string soundName)
        {
            Sound sound = FindSound(groupName, soundName);

            sound.source.Stop();
        }
    }
}
