using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSettings.Inputs.Basic
{
    public abstract class SettingsInput : MonoBehaviour, IPointerEnterHandler
    {
        [field: SerializeField] public string SettingName { get; protected set; }

        [SerializeField] private Sprite _tooltipCover;
        [SerializeField, TextArea(5, 5)] private string _tooltipDescription;
        [SerializeField] private bool _confirmOnChange;

        protected Stack<object> _valueHistory = new Stack<object>();
        protected Settings _settings;

        public virtual void Setup() { }
        public abstract void ResetToDefault();
        public abstract object Save();
        public abstract void Load(object data);
        public abstract void PreviousOption();
        public abstract void NextOption();

        public void SetSettings(Settings settings)
        {
            _settings = settings;
        }
        public Sprite GetTooltipCoverSprite() => _tooltipCover;
        public string GetTooltipDescription() => _tooltipDescription;

        public virtual void RevertLast()
        {
            // Skip first element, to get last value before confirmation
            _valueHistory.Pop();

            if (_confirmOnChange)
                _settings.ConfirmSetting();
        }

        public void OnValueChanged()
        {
            if (_confirmOnChange)
                NotifyConfirmation();

            SaveLastValue();
        }

        private void SaveLastValue()
        {
            _valueHistory.Push(Save());
        }

        public void NotifyConfirmation()
        {
            _settings.ShowConfirmation(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _settings.ShowTooltip(this);
        }
    }
}
