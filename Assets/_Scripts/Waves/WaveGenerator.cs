using ObjectPooling;
using Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                enemy.Die();

            _enemiesAlive.Clear();
            _isStopped = true;
        }

        public void OnEnable()
        {
            EnemyBehaviour.OnEnemyDied += RemoveAliveEnemy;
        }

        public void OnDisable()
        {
            EnemyBehaviour.OnEnemyDied -= RemoveAliveEnemy;
        }

        private void RemoveAliveEnemy(EnemyBehaviour enemy)
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

                SpawnEnemy(entry.Enemy, 0);
                await Task.Delay(TimeSpan.FromSeconds(entry.GetIntervalByWave(wave)));
            }
        }

        private void SpawnEnemy(EnemyData enemyData, int nextWaypointIndex)
        {
            GameObject enemyObject = PoolManager.Instance.SpawnFromPool("Enemy", _origin.position, Quaternion.identity);
            EnemyBehaviour enemy = EnemiesCache.GetEnemyByGameObject(enemyObject);

            enemy.Setup(enemyData, _path, nextWaypointIndex);
            _enemiesAlive.Add(enemy);
        }
    }
}
