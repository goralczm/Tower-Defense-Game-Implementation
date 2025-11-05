using BuildingSystem.Core;
using System;
using TooltipSystem;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem.UI
{
    public class BuildingSlot : TooltipTrigger
    {
        [Header("References")]
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;

        private IBuilding _building;

        public override string Header => _building?.Name ?? "";
        public override string Content => _building?.Description ?? "";

        public void Setup(IBuilding building, Action buttonAction)
        {
            _building = building;
            _icon.sprite = _building.Sprite;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => buttonAction());
        }
    }
}
