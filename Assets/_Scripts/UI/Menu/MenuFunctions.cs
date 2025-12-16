using Core.Systems;
using UnityEngine;

namespace UI.Menu
{
    public class MenuFunctions : MonoBehaviour
    {
        public void StartNewGame()
        {
            GlobalSystems.Instance.LevelSettings.LoadGame = false;
            GlobalSystems.Instance.SceneManager.ChangeScene("level1");
        }

        public void ShowSettings()
        {
            GlobalSystems.Instance.Settings.Show();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
