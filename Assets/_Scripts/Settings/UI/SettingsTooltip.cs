using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameSettings.UI
{
    public class SettingsTooltip : MonoBehaviour
    {
        [Header("Instances")]
        [SerializeField] private Image _coverImage;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _settingNameText;

        public void SetCover(Sprite coverSprite)
        {
            if (coverSprite == null)
                _coverImage.gameObject.SetActive(false);
            else
            {
                _coverImage.sprite = coverSprite;
                _coverImage.gameObject.SetActive(true);
            }
        }

        public void SetSettingName(string settingName)
        {
            _settingNameText.SetText(settingName);
        }

        public void SetDescription(string description)
        {
            _descriptionText.SetText(description);
        }
    }
}
