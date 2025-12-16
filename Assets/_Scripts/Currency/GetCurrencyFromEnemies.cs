using Core;
using DG.Tweening;
using Enemies;
using UnityEngine;

namespace Currency
{
    public class GetCurrencyFromEnemies : MonoBehaviour
    {
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
            if (reason != DeathReason.External)
                return;

            Bank.Instance.AddCurrency(enemy.EnemyData.Reward);
        }

        /*private void FlyingCoins(EnemyBehaviour enemy)
        {
            for (int i = 0; i < enemy.EnemyData.Reward; i++)
            {
                var coin = PoolManager.Instance.SpawnFromPool(
                    "Coin",
                    (Vector2)enemy.transform.position + Random.insideUnitCircle * .2f,
                    Quaternion.identity);

                coin.transform
                    .FlyTowards(
                        coin.transform.position.Add(x: Random.Range(-.5f, .5f), y: -.5f),
                        Ease.OutBounce,
                        1f,
                        () =>
                        {
                            coin.transform
                                .FlyTowards(
                                    Camera.main.ScreenToWorldPoint(_currencyText.position),
                                    Ease.InBack,
                                    Random.Range(.5f, 1f),
                                    () =>
                                    {
                                        coin.SetActive(false);
                                        Bank.Instance.AddCurrency(1);
                                        _currencyText.SimplePunch();
                                    });
                        });
            }
        }*/
    }
}
