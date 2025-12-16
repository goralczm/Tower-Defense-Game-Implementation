using Core.Systems;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Menu
{
    public class ContinueGameButton : MonoBehaviour
    {
        [SerializeField] private Image _heroImage;
        [SerializeField] private GameObject _blocker;

        private void Start()
        {
            bool hasSavedGame = HasSavedGame();

            _blocker.SetActive(!hasSavedGame);

            if (hasSavedGame)
                TryLoadHeroImage();
        }

        private void TryLoadHeroImage()
        {
            var path = Path.Combine(Application.persistentDataPath, "save_screen.png");
            if (!File.Exists(path))
                return;

            try
            {
                var bytes = File.ReadAllBytes(path);
                var texture = new Texture2D(2, 2);
                if (!texture.LoadImage(bytes))
                    return;

                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                _heroImage.sprite = sprite;
                _heroImage.preserveAspect = true;
            }
            catch (IOException)
            {

            }
        }

        private bool HasSavedGame()
        {
            return SaveSystem.SaveSystem.SaveDataExists("level1_MapData");
        }

        public void ContinueGame()
        {
            GlobalSystems.Instance.LevelSettings.LoadGame = true;
            GlobalSystems.Instance.SceneManager.ChangeScene("level1");
        }
    }
}
