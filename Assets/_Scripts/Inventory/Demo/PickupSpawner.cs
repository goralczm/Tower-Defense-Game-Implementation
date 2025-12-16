using Core;
using Enemies;
using ObjectPooling;
using System.Linq;
using Towers.Projectiles;
using UnityEngine;
using Utilities;
using Utilities.Extensions;

namespace Inventory.Demo
{
    public class PickupSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _spawnPossibility = .1f;
        [SerializeField] private float _spawnCooldown = 1f;
        [SerializeField] private ProjectileData[] _pool;

        private float _nextSpawnTime;

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
            if (reason != DeathReason.External)
                return;

            if (_nextSpawnTime > Time.time)
                return;

            if (!Randomizer.GetRandomBool(_spawnPossibility))
                return;

            PickupFactory.CreatePickup(GetRandomProjectile(), enemy.transform.position);

            _nextSpawnTime = Time.time + _spawnCooldown;
        }

        private IItem GetRandomProjectile()
        {
            return _pool.Where(p => p.Rarity > Rarity.Common).ToList().GetRandom(1)[0];
        }
    }
}
