using UnityEngine;
using Waves;

namespace Currency
{
    public class GetCurrencyFromWaves : MonoBehaviour
    {
        private void OnEnable()
        {
            WavesController.OnWaveEnded += OnWaveEnded;
        }

        private void OnDisable()
        {
            WavesController.OnWaveEnded -= OnWaveEnded;
        }

        private void OnWaveEnded(int currentWave, int wavesCount, WavesData wavesData)
        {
            if (currentWave < 1)
                return;

            Bank.Instance.AddCurrency(wavesData.Reward);
        }
    }
}
