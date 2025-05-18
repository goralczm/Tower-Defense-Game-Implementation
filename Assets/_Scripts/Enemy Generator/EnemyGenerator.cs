using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private float _intervals;

    private Dictionary<GameObject, Enemy> _enemyCache = new();

    private void Start()
    {
        PathDisplay.OnPathGenerated += StartSpawner;
    }

    private void StartSpawner()
    {
        InvokeRepeating("SpawnEnemy", 0, _intervals);
        PathDisplay.OnPathGenerated -= StartSpawner;
    }

    public void SpawnEnemy()
    {
        GameObject enemyObject = PoolManager.Instance.SpawnFromPool("Enemy", transform.position, Quaternion.identity);
        if (!_enemyCache.TryGetValue(enemyObject, out Enemy enemy))
        {
            enemy = enemyObject.GetComponent<Enemy>();
            _enemyCache.Add(enemyObject, enemy);
        }

        enemy.ResetCache();
    }
}
