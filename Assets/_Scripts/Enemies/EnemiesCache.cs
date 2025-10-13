using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemiesCache
    {
        private static Dictionary<Collider2D, EnemyBehaviour> _enemiesByCollider = new();
        private static Dictionary<GameObject, EnemyBehaviour> _enemiesByGameObject = new();

        public static EnemyBehaviour GetEnemyByCollider(Collider2D collider)
        {
            if (!_enemiesByCollider.TryGetValue(collider, out EnemyBehaviour enemy))
            {
                enemy = collider.GetComponent<EnemyBehaviour>();
                _enemiesByCollider.Add(collider, enemy);
            }

            return enemy;
        }

        public static EnemyBehaviour GetEnemyByGameObject(GameObject gameObject)
        {
            if (!_enemiesByGameObject.TryGetValue(gameObject, out EnemyBehaviour enemy))
            {
                enemy = gameObject.GetComponent<EnemyBehaviour>();
                _enemiesByGameObject.Add(gameObject, enemy);
            }

            return enemy;
        }
    }
}
