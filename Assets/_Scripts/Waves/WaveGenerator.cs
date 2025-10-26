using Core;
using ObjectPooling;
using Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Waves;

namespace Enemies
{
    public class WaveGenerator
    {
        private HashSet<EnemyBehaviour> _enemiesAlive = new();
        private Transform _origin;
        private WavesData _waves;
        private Path _path;
        private bool _isStopped;

        public bool IsStopped => _isStopped;
        public int EnemiesAliveCount => _enemiesAlive.Count;

        public WaveGenerator(WavesData waves, Path path, Transform origin)
        {
            _waves = waves;
            _path = path;
            _origin = origin;
        }

        public void UnstopGenerator()
        {
            _isStopped = false;
        }

        public void StopGenerator()
        {
            var enemiesCopy = _enemiesAlive.ToList();
            foreach (var enemy in enemiesCopy)
                enemy.Die(DeathReason.Self);

            _enemiesAlive.Clear();
            _isStopped = true;
        }

        public void OnEnable()
        {
            EnemyBehaviour.OnEnemyDied += RemoveAliveEnemy;
            EnemyBehaviour.SpawnEnemyRequest += SpawnEnemy;
        }

        public void OnDisable()
        {
            EnemyBehaviour.OnEnemyDied -= RemoveAliveEnemy;
            EnemyBehaviour.SpawnEnemyRequest -= SpawnEnemy;
        }

        private void RemoveAliveEnemy(EnemyBehaviour enemy, DeathReason reason)
        {
            _enemiesAlive.Remove(enemy);
        }

        public async Task StartGenerator(int wave)
        {
            if (IsStopped) return;

            _enemiesAlive.Clear();
            foreach (var entry in _waves.Entries)
            {
                await SpawnEnemiesByEntry(entry, wave);
            }
        }

        private async Task SpawnEnemiesByEntry(WaveEntry entry, int wave)
        {
            int enemiesCount = entry.GetCountByWave(wave);
            for (int i = 0; i < enemiesCount; i++)
            {
                if (IsStopped) return;

#if UNITY_EDITOR
                if (EditorApplication.isPaused)
                    await Task.Delay(10);
#endif

                SpawnEnemy(entry.Enemy, 0, _origin.position);
                await Task.Delay(TimeSpan.FromSeconds(entry.GetIntervalByWave(wave)));
            }
        }

        public void SpawnEnemy(EnemyData enemyData, int nextWaypointIndex, Vector2 position)
        {
            GameObject enemyObject = PoolManager.Instance.SpawnFromPool("Enemy", position, Quaternion.identity);
            EnemyBehaviour enemy = EnemiesCache.GetEnemyByGameObject(enemyObject);

            enemy.Setup(enemyData, _path, nextWaypointIndex);
            _enemiesAlive.Add(enemy);
        }
    }
}
