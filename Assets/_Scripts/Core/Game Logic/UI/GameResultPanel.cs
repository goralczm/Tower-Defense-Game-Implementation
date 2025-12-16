using Core;
using TMPro;
using UI;
using UnityEngine;

namespace GameLogic
{
    public class GameResultPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UITweener _panel;
        [SerializeField] private TextMeshProUGUI _resultText;

        private void OnEnable()
        {
            GlobalGameEvents.OnGameEnded += OnGameEnded;
        }

        private void OnDisable()
        {
            GlobalGameEvents.OnGameEnded -= OnGameEnded;
        }

        private void OnGameEnded(bool result)
        {
            _panel.Show();

            if (result)
                _resultText.SetText("Victory!");
            else
                _resultText.SetText("Lost!");
        }
    }
}
