using TMPro;
using UnityEngine;

namespace Currency
{
    public class CurrencyDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currencyText;

        private void OnEnable()
        {
            Bank.OnCurrencyChanged += UpdateCurrencyText;
        }

        private void OnDisable()
        {
            Bank.OnCurrencyChanged -= UpdateCurrencyText;
        }

        private void UpdateCurrencyText(int currency)
        {
            _currencyText.SetText($"Currency: {currency}");
        }
    }
}
