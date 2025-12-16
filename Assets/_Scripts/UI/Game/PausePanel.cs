using Core.Systems;
using UnityEngine;

namespace UI.Game
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] private UITweener _tweener;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePause();
        }

        public void TogglePause()
        {
            Time.timeScale = Time.timeScale == 0 ? 1f : 0f;
            if (Time.timeScale == 0)
                _tweener.Show();
            else
                _tweener.Hide();
        }

        public void RestartLevel()
        {
            GlobalSystems.Instance.LevelSettings.LoadGame = false;
            GlobalSystems.Instance.SceneManager.ChangeScene("level1");
        }

        public void BackToMenu()
        {
            GlobalSystems.Instance.SceneManager.ChangeScene("menu");
        }

        public void ShowSettings()
        {
            GlobalSystems.Instance.Settings.Show();
        }
    }
}
