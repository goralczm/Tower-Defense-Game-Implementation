using Core;
using Enemies;
using UnityEngine;
using Utilities;

namespace Currency
{
    public class Bank : Singleton<Bank>
    {
        [SerializeField] private int _currency;

        private void OnEnable()
        {
            EnemyBehaviour.OnEnemyDied += this.OnEnemyDied;
        }

        private void OnDisable()
        {
            EnemyBehaviour.OnEnemyDied -= this.OnEnemyDied;
        }

        private void OnEnemyDied(EnemyBehaviour enemy, DeathReason reason)
        {
            if (reason != DeathReason.External) return;

            _currency += enemy.EnemyData.Reward;
        }
    }
}
