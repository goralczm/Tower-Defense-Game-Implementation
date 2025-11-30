using Core.Systems;
using UnityEngine;
using Waves;

namespace GameLogic
{
    public class SaveGameOnWaveEnded : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSetup _gameSetup;

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
            if (!_gameSetup.SetupDone)
                return;

            if (currentWave <= wavesCount)
                _gameSetup.Save();
        }
    }
}
