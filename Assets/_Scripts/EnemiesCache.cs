using System.Collections.Generic;
using UnityEngine;

public class EnemiesCache
{
    private static Dictionary<Collider2D, Enemy> _enemiesCache = new();

    public static Enemy GetEnemyByCollider(Collider2D collider)
    {
        if (!_enemiesCache.TryGetValue(collider, out Enemy enemy))
        {
            enemy = collider.GetComponent<Enemy>();
            _enemiesCache.Add(collider, enemy);
        }

        return enemy;
    }
}