using Core;
using Enemies;
using ObjectPooling;
using Towers.Projectiles;
using UnityEngine;
using Utilities;
using Utilities.Extensions;

namespace Inventory.Demo
{
    public class EnemyPickupSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _spawnPossibility = .1f;
        [SerializeField] private float _spawnCooldown = 1f;

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
            if (_nextSpawnTime > Time.time) return;

            if (!Randomizer.GetRandomBool(_spawnPossibility)) return;

            PickupFactory.CreatePickup(GetRandomProjectile(), enemy.transform.position);

            _nextSpawnTime = Time.time + _spawnCooldown;
        }

        private IItem GetRandomProjectile()
        {
            var projectiles = Resources.LoadAll<ProjectileData>("Projectiles/Droppable");

            return projectiles.GetRandom(1)[0];
        }
    }
}
