using UnityEngine;

namespace Core.Systems
{
    public class SceneManager : MonoBehaviour
    {
        private string _sceneToLoad;

        public void ChangeScene(string sceneName)
        {
            _sceneToLoad = sceneName;
            Time.timeScale = 1f;
            GlobalSystems.Instance.TransitionController.DoTransition(ChangeSceneInternal);
        }

        private void ChangeSceneInternal()
        {
            if (string.IsNullOrEmpty(_sceneToLoad))
                return;

            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
            _sceneToLoad = string.Empty;
        }
    }
}
