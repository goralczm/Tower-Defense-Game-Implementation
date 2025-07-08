using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _intervals;

    private Dictionary<GameObject, Enemy> _enemyCache = new();

    private void Start()
    {
        PathGenerationOrchestrator.OnPathGenerationStarted += StopSpawner;
        PathGenerationOrchestrator.OnPathGenerationEnded += MoveSpawner;
        PathGenerationOrchestrator.OnPathGenerationEnded += StartSpawner;
    }

    private void OnDisable()
    {
        PathGenerationOrchestrator.OnPathGenerationStarted -= StopSpawner;
        PathGenerationOrchestrator.OnPathGenerationEnded -= MoveSpawner;
        PathGenerationOrchestrator.OnPathGenerationEnded -= StartSpawner;
    }
    
    private void StopSpawner(object sender, EventArgs e)
    {
        CancelInvoke();
    }

    private void MoveSpawner(object sender, PathGenerationOrchestrator.OnPathGeneratedEventArgs args)
    {
        transform.position = args.StartPointWorld;
    }


    private void StartSpawner(object sender, PathGenerationOrchestrator.OnPathGeneratedEventArgs args)
    {
        InvokeRepeating("SpawnEnemy", 0, _intervals);
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
