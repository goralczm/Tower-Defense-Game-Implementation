using Core;
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
        private bool _hasGameEnded;

        public static event Action<int, int, WavesData> OnWaveEnded;

        public int CurrentWave => _currentWave;
        public int WavesCount => _waves.WavesCount;

        private void OnEnable()
        {
            _generator = new(_waves, _path, transform);
            _generator.OnEnable();
            GlobalGameEvents.OnGameEnded += OnGameEnded;
        }

        private void OnDisable()
        {
            _generator.OnDisable();
            GlobalGameEvents.OnGameEnded -= OnGameEnded;
        }

        private void Start()
        {
            OnWaveEnded?.Invoke(CurrentWave, WavesCount, _waves);
        }

        public async Task StartGenerator()
        {
            if (_generator.IsStopped || _generator.IsRunning)
                return;

            await _generator.StartGenerator(_currentWave);

            await Helpers.WaitUntilAsync(() => _generator.EnemiesAliveCount == 0);

            if (_hasGameEnded)
                return;

            FinishWave();
            Debug.Log("Wave finished!");
        }

        private void FinishWave()
        {
            OnWaveEnded?.Invoke(++_currentWave, WavesCount, _waves);
        }

        private void OnGameEnded(bool result)
        {
            _hasGameEnded = true;
            _generator.StopGenerator();
            gameObject.SetActive(false);
        }
    }
}
