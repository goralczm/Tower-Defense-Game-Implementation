using Attributes;
using Enemies;
using UnityEngine;

namespace Core.Player
{
    public class DamagePlayerWhenEnemyReachEnd : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Player _player;

        private void OnEnable()
        {
            EnemyBehaviour.OnEnemyDied += OnEnemyDied;
        }

        private void OnDisable()
        {
            EnemyBehaviour.OnEnemyDied -= OnEnemyDied;
        }

        private void OnEnemyDied(EnemyBehaviour enemy, DeathReason reason)
        {
            if (reason != DeathReason.Self)
                return;

            _player.TakeDamage(enemy.Attributes.GetAttribute(EnemyAttributes.Damage, 1), new DamageType[0], enemy.EnemyData.name);
        }
    }
}
