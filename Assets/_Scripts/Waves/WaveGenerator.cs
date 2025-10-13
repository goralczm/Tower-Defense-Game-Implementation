using ObjectPooling;
using Paths;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Waves;

namespace Enemies
{
    public class WaveGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private WaveData _wave;

        [Header("References")]
        [SerializeField] private Path _path;

        // Set for faster lookup and removal
        private List<EnemyBehaviour> _enemiesAlive = new();
        private bool _isStopped;
        private CancellationTokenSource _cts = new();

        public bool IsStopped => _isStopped;

        public void UnstopGenerator()
        {
            _isStopped = false;
        }

        public void StopGenerator()
        {
            _cts.Cancel();

            for (int i = _enemiesAlive.Count - 1; i >= 0; i--)
                _enemiesAlive[i].Die(notify: false);

            _enemiesAlive.Clear();

            _isStopped = true;
        }

        private void OnEnable()
        {
            EnemyBehaviour.OnEnemyDied += RemoveAliveEnemy;
        }

        private void OnDisable()
        {
            EnemyBehaviour.OnEnemyDied -= RemoveAliveEnemy;
        }

        private void RemoveAliveEnemy(EnemyBehaviour enemy)
        {
            if (_enemiesAlive.Contains(enemy))
                _enemiesAlive.Remove(enemy);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (IsStopped)
                    UnstopGenerator();
                else
                    StopGenerator();
            }
        }

        public async Task StartGenerator(int wave)
        {
            if (IsStopped) return;

            _cts = new();
            await SpawnWave(wave);
        }

        private async Task SpawnWave(int wave)
        {
            _enemiesAlive.Clear();
            foreach (var entry in _wave.Entries)
            {
                await SpawnEnemiesByEntry(entry, wave);
            }
        }

        private async Task SpawnEnemiesByEntry(WaveEntry entry, int wave)
        {
            int enemiesCount = entry.GetCountByWave(wave);
            for (int i = 0; i < enemiesCount; i++)
            {
                _cts.Token.ThrowIfCancellationRequested();

                SpawnEnemy(entry.Enemy, 0);
                await Task.Delay(TimeSpan.FromSeconds(entry.GetIntervalByWave(wave)));
            }
        }

        private void SpawnEnemy(EnemyData enemyData, int nextWaypointIndex)
        {
            GameObject enemyObject = PoolManager.Instance.SpawnFromPool("Enemy", transform.position, Quaternion.identity);
            EnemyBehaviour enemy = EnemiesCache.GetEnemyByGameObject(enemyObject);

            enemy.Setup(enemyData, _path, nextWaypointIndex);
            _enemiesAlive.Add(enemy);
        }
    }
}
