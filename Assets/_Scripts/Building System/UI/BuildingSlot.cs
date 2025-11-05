using BuildingSystem.Core;
using System;
using System.Linq;
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

        public override string Header => _building.GetType().ToString().Split(".").Last();
        public override string Content => "";

        public void Setup(IBuilding building, Action buttonAction)
        {
            _building = building;
            _icon.sprite = _building.Sprite;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => buttonAction());
        }
    }
}
