using Core;
using UI;
using UnityEngine;

namespace GameLogic
{
    public class GameResultPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UITweener _panel;

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
        }
    }
}
