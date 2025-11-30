using TMPro;
using UnityEngine;

namespace Waves
{
    public class WavesDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _wavesText;

        private void OnEnable()
        {
            WavesController.OnWaveChanged += OnWaveChanged;
        }

        private void OnDisable()
        {
            WavesController.OnWaveChanged -= OnWaveChanged;
        }

        private void OnWaveChanged(int currentWave, int wavesCount)
        {
            _wavesText.SetText($"Wave: {currentWave}/{wavesCount}");
        }
    }
}
