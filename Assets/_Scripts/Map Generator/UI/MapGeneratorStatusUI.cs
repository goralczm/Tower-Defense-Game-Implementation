using TMPro;
using UnityEngine;

namespace MapGenerator.UI
{
    public class MapGeneratorStatusUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapGenerator.Demo.MapGenerator _mapGenerator;
        [SerializeField] private GameObject _generationPanel;
        [SerializeField] private TextMeshProUGUI _statusText;

        private void OnEnable()
        {
            _mapGenerator.OnMapGenerationStarted += ShowGenerationPanel;
            _mapGenerator.OnMapGenerationEnded += HideGenerationPanelOnEnd;
            _mapGenerator.OnMapGenerationCanceled += HideGenerationPanelOnCancel;
            _mapGenerator.OnStatusChanged += UpdateStatusText;
        }

        private void OnDisable()
        {
            _mapGenerator.OnMapGenerationStarted -= ShowGenerationPanel;
            _mapGenerator.OnMapGenerationEnded -= HideGenerationPanelOnEnd;
            _mapGenerator.OnMapGenerationCanceled -= HideGenerationPanelOnCancel;
            _mapGenerator.OnStatusChanged -= UpdateStatusText;
        }

        private void HideGenerationPanelOnEnd(object sender, Demo.MapGenerator.OnMapGeneratedEventArgs e)
        {
            _generationPanel.SetActive(false);
        }

        private void HideGenerationPanelOnCancel(object sender, System.EventArgs e)
        {
            _generationPanel.SetActive(false);
        }

        private void ShowGenerationPanel(object sender, System.EventArgs e)
        {
            _generationPanel.SetActive(true);
        }

        private void UpdateStatusText(string status)
        {
            _statusText.SetText(status);
        }
    }
}
