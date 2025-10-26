using Enemies;
using Paths;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace Waves
{
    public class WavesController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private WavesData _waves;
        [SerializeField] private int _currentWave = 1;

        [Header("References")]
        [SerializeField] private Path _path;

        private WaveGenerator _generator;

        public static event Action<int, int> OnWaveEnded;

        public int CurrentWave => _currentWave;
        public int WavesCount => _waves.WavesCount;

        private void OnEnable()
        {
            _generator = new(_waves, _path, transform);
            _generator.OnEnable();
            OnWaveEnded?.Invoke(CurrentWave, WavesCount);
        }

        private void OnDisable()
        {
            _generator.OnDisable();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (_generator.IsStopped)
                    _generator.UnstopGenerator();
                else
                    _generator.StopGenerator();
            }
        }

        public async Task StartGenerator()
        {
            await _generator.StartGenerator(_currentWave);

            await WaitUntilAsync(() => _generator.EnemiesAliveCount == 0);

            FinishWave();
            Debug.Log("Wave finished!");
        }

        private void FinishWave()
        {
            // TODO: Give money
            _currentWave++;
            OnWaveEnded?.Invoke(_currentWave, WavesCount);
        }

        private async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
        {
            while (!condition())
                await Task.Delay(checkIntervalMs);
        }
    }
}
