using UnityEngine;

namespace Core.Systems
{
    public class TransitionTest : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                GlobalSystems.Instance.SceneManager.ChangeScene("menu");

            if (Input.GetKeyDown(KeyCode.J))
                GlobalSystems.Instance.SceneManager.ChangeScene("level1");
        }
    }
}
