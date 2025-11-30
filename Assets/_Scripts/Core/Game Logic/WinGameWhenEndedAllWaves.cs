using Core;
using UnityEngine;
using Waves;

namespace GameLogic
{
    public class WinGameWhenEndedAllWaves : MonoBehaviour
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
            if (currentWave > wavesCount)
                GlobalGameEvents.OnGameEnded?.Invoke(true);
        }
    }
}
