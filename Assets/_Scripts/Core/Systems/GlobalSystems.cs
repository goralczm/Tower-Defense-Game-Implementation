using GameSettings;
using Utilities;

namespace Core.Systems
{
    [System.Serializable]
    public class LevelSettings
    {
        public bool LoadGame;
    }

    public class GlobalSystems : Singleton<GlobalSystems>
    {
        public AudioSystem.AudioSystem AudioSystem;
        public Settings Settings;
        public TransitionController TransitionController;
        public SceneManager SceneManager;
        public LevelSettings LevelSettings;
    }
}
