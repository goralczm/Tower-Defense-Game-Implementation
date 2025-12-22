using Attributes;
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
using Utilities;
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
        private bool _isRunning;

        public bool IsStopped => _isStopped;
        public int EnemiesAliveCount => _enemiesAlive.Count;
        public bool IsRunning => _isRunning || EnemiesAliveCount > 0;

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
            if (IsStopped)
                return;

            _isRunning = true;
            _enemiesAlive.Clear();
            foreach (var entry in _waves.Entries)
            {
                await SpawnEnemiesByEntry(entry, wave);
            }
            _isRunning = false;
        }

        private async Task SpawnEnemiesByEntry(WaveEntry entry, int wave)
        {
            float additionalHealth = entry.HealthIncrement * (wave + 1) / 10;

            int enemiesCount = entry.GetCountByWave(wave);
            for (int i = 0; i < enemiesCount; i++)
            {
                if (IsStopped) return;

                if (Time.timeScale == 0)
                    await Helpers.WaitUntilAsync(() => Time.timeScale != 0);

                var enemy = SpawnEnemyInternal(entry.Enemy, 0, _origin.position);

                if (additionalHealth > 0)
                    enemy.Attributes.Mediator.AddModifier(new MathAttributeModifier<EnemyAttributes>(EnemyAttributes.Health, 0f, MathOperation.Add, additionalHealth));

                await Task.Delay(TimeSpan.FromSeconds(entry.GetIntervalByWave(wave)));
            }
        }

        public void SpawnEnemy(EnemyData enemyData, int nextWaypointIndex, Vector2 position)
        {
            SpawnEnemyInternal(enemyData, nextWaypointIndex, position);
        }

        public EnemyBehaviour SpawnEnemyInternal(EnemyData enemyData, int nextWaypointIndex, Vector2 position)
        {
            GameObject enemyObject = PoolManager.Instance.SpawnFromPool("Enemy", position, Quaternion.identity);
            EnemyBehaviour enemy = EnemiesCache.GetEnemyByGameObject(enemyObject);

            enemy.Setup(enemyData, _path, nextWaypointIndex);
            _enemiesAlive.Add(enemy);

            return enemy;
        }
    }
}
