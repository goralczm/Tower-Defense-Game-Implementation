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
            if (!Randomizer.GetRandomBool(.5f)) return;

            var pickupObject = PoolManager.Instance.SpawnFromPool("Pickup", enemy.transform.position, Quaternion.identity);
            var pickup = pickupObject.GetComponent<Pickup>();

            pickup.Setup(GetRandomProjectile());
        }

        private IItem GetRandomProjectile()
        {
            var projectiles = Resources.LoadAll<ProjectileData>("Projectiles/Droppable");

            return projectiles.GetRandom(1)[0];
        }
    }
}
