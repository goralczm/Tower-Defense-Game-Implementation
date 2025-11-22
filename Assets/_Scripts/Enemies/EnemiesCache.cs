using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemiesCache
    {
        private static Dictionary<Collider2D, EnemyBehaviour> _enemiesByCollider = new();
        private static Dictionary<GameObject, EnemyBehaviour> _enemiesByGameObject = new();
        private static Dictionary<Transform, EnemyBehaviour> _enemiesByTransform = new();

        public static bool TryGetEnemy(Transform transform, out EnemyBehaviour enemy)
        {
            enemy = GetEnemyByTransform(transform);

            return enemy != null;
        }

        public static bool TryGetEnemy(Collider2D collider, out EnemyBehaviour enemy)
        {
            enemy = GetEnemyByCollider(collider);

            return enemy != null;
        }

        public static bool TryGetEnemy(GameObject gameObject, out EnemyBehaviour enemy)
        {
            enemy = GetEnemyByGameObject(gameObject);

            return enemy != null;
        }

        public static EnemyBehaviour GetEnemyByTransform(Transform transform)
        {
            if (!_enemiesByTransform.TryGetValue(transform, out EnemyBehaviour enemy))
            {
                enemy = transform.GetComponent<EnemyBehaviour>();
                _enemiesByTransform.Add(transform, enemy);
            }

            return enemy;
        }

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
