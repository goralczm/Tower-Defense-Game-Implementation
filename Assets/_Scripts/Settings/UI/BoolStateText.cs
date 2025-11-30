using TMPro;
using UnityEngine;

namespace GameSettings.UI
{
    public class BoolStateText : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string _onText;
        [SerializeField] private string _offText;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _text;

        public void UpdateText(bool isOn)
        {
            if (isOn)
                _text.SetText(_onText);
            else
                _text.SetText(_offText);
        }
    }
}
