using BuildingSystem.Core;
using System;
using TMPro;
using TooltipSystem;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem.UI
{
    public class BuildingSlot : TooltipTrigger
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;

        private IBuilding _building;

        public override string Header => _building?.Name ?? "";
        public override string Content => _building?.Description ?? "";

        public void Setup(IBuilding building, Action buttonAction)
        {
            _building = building;
            _icon.sprite = _building.Sprite;
            _icon.color = _building.Color;
            _nameText.SetText(_building.Name);
            _costText.SetText(_building.Cost.ToString());
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => buttonAction());
        }
    }
}
