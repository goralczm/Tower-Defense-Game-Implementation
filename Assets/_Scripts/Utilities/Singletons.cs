using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Class that provides static instance of given type.
    /// </summary>
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }
        protected virtual void Awake() => Instance = this as T;

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Class derives from StaticInstance class, but with addition of destroying itself, when other instance is present.
    /// </summary>
    public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();
        }
    }

    /// <summary>
    /// Class derives from Singleton class, but with addition of not being destroyed on changing scene.
    /// </summary>
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}
