using ObjectPooling;
using UnityEngine;
using Utilities;

namespace VisualEffects
{
    public class ParticlesOnDisable : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string _particlesTag;

        private void OnDisable()
        {
            if (AppLifecycle.IsQuitting) return;

            PoolManager.Instance.SpawnFromPool(_particlesTag, transform.position, Quaternion.identity);
        }
    }
}
