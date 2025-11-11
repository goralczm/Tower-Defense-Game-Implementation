using Core;
using DG.Tweening;
using Enemies;
using ObjectPooling;
using UI;
using UnityEngine;
using Utilities.Extensions;
using static UnityEngine.UI.Image;

namespace Currency
{
    public class EnemyCurrencySpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _currencyText;

        private void OnEnable()
        {
            EnemyBehaviour.OnEnemyDied += OnEnemyDied;
        }

        private void OnDisable()
        {
            EnemyBehaviour.OnEnemyDied -= OnEnemyDied;
        }

        private void OnEnemyDied(EnemyBehaviour enemy, DeathReason reason)
        {
            if (reason != DeathReason.External) return;

            Bank.Instance.AddCurrency(enemy.EnemyData.Reward);
            Tweens.Punch(_currencyText);
        }

        private void FlyingCoins(EnemyBehaviour enemy)
        {
            for (int i = 0; i < enemy.EnemyData.Reward; i++)
            {
                var coin = PoolManager.Instance.SpawnFromPool(
                    "Coin",
                    (Vector2)enemy.transform.position + Random.insideUnitCircle * .2f,
                    Quaternion.identity);

                Tweens.FlyTowards(
                    coin.transform,
                    coin.transform.position.Add(x: Random.Range(-.5f, .5f), y: -.5f),
                    Ease.OutBounce,
                    1f,
                    () =>
                    {
                        Tweens.FlyTowards(
                            coin.transform,
                            Camera.main.ScreenToWorldPoint(_currencyText.position),
                            Ease.InBack,
                            Random.Range(.5f, 1f),
                            () =>
                            {
                                coin.SetActive(false);
                                Bank.Instance.AddCurrency(1);
                                Tweens.Punch(_currencyText);
                            });
                    });
            }
        }
    }
}
