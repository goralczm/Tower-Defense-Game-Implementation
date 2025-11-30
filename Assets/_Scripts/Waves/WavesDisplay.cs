using TMPro;
using UnityEngine;

namespace Waves
{
    public class WavesDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _wavesText;

        private void OnEnable()
        {
            WavesController.OnWaveEnded += UpdateWavesText;
        }

        private void OnDisable()
        {
            WavesController.OnWaveEnded -= UpdateWavesText;
        }

        private void UpdateWavesText(int currentWave, int wavesCount, WavesData wavesData)
        {
            _wavesText.SetText($"Wave: {currentWave}/{wavesCount}");
        }
    }
}
