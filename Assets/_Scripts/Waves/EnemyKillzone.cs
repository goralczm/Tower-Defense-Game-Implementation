using Enemies;
using UnityEngine;

namespace Waves
{
    public class EnemyKillzone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (EnemiesCache.TryGetEnemy(collision, out EnemyBehaviour enemy))
                enemy.Die();
        }
    }
}
