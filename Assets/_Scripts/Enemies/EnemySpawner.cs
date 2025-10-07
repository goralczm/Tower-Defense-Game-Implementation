using ObjectPooling;
using Paths;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _intervals;

        [Header("References")]
        [SerializeField] private Path _path;

        private Dictionary<GameObject, Enemy> _enemyCache = new();

        private bool _isStopped = true;

        public bool IsStopped => _isStopped;

        public void StartSpawner()
        {
            InvokeRepeating("SpawnEnemy", 0, _intervals);
            _isStopped = false;
        }

        public void StopSpawner()
        {
            CancelInvoke();
            _isStopped = true;
        }

        public void SpawnEnemy()
        {
            GameObject enemyObject = PoolManager.Instance.SpawnFromPool("Enemy", transform.position, Quaternion.identity);
            if (!_enemyCache.TryGetValue(enemyObject, out Enemy enemy))
            {
                enemy = enemyObject.GetComponent<Enemy>();
                _enemyCache.Add(enemyObject, enemy);
            }

            enemy.Reset();
            enemy.SetPath(_path);
        }
    }
}
