using System;
using System.Linq;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using Utilities;

namespace Core.Systems
{
    public class TransitionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UITweener[] _transitionTweeners;

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            OnEnter();
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            OnEnter();
        }

        private void OnEnter()
        {
            foreach (var tween in _transitionTweeners)
                tween.Hide();
        }

        public async Task DoTransition(Action onEndHandler)
        {
            foreach (var tween in _transitionTweeners)
            {
                tween.Kill();
                tween.Show();
            }

            await Helpers.WaitUntilAsync(() => _transitionTweeners.All(t => !t.IsRunning));

            onEndHandler?.Invoke();
        }
    }
}
