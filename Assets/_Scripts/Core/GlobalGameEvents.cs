using System;
using UnityEngine;

namespace Core
{
    public static class GlobalGameEvents
    {
        public static Action<bool> OnGameEnded;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Clear()
        {
            OnGameEnded = null;
        }
    }
}
