using UnityEngine;

namespace Utilities
{
    [DefaultExecutionOrder(-1000)]
    public class AppLifecycle : MonoBehaviour
    {
        private static bool _isQuitting;

        public static bool IsQuitting => _isQuitting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetQuitFlag() => _isQuitting = false;

        private void OnApplicationQuit() => _isQuitting = true;
    }
}
