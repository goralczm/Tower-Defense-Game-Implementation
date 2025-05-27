using System;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace AudioSystem
{
    /// <summary>
    /// Represents a sound group along with mixer group and cached sounds associated with this group.
    /// </summary>
    [System.Serializable]
    public class SoundGroup
    {
        public string name;
        public Sound[] sounds;
        public AudioMixerGroup mixerGroup;

        private Dictionary<string, Sound> _sounds = new Dictionary<string, Sound>();

        /// <summary>
        /// Caches the provided <see cref="Sound"/>.
        /// </summary>
        /// <param name="sound">The sound to be cached.</param>
        /// <exception cref="ArgumentException">Thrown when sound name already exists in the cached dictionary.</exception>
        public void AddSoundToDicitonary(Sound sound)
        {
            if (_sounds.ContainsKey(sound.name))
                throw new ArgumentException($"{sound.name} already exists in {name} sound group!");

            _sounds.Add(sound.name, sound);
        }

        /// <summary>
        /// Retrieves the cached sound from dictionary based on the provided sound name.
        /// </summary>
        /// <param name="soundName">The sound name.</param>
        /// <returns>The associated <see cref="Sound"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the requested sound name is not found in the cache.</exception>
        public Sound GetSound(string soundName)
        {
            if (!_sounds.ContainsKey(soundName))
                throw new KeyNotFoundException($"{name}: {soundName} could not be found!");

            return _sounds[soundName];
        }
    }
}
