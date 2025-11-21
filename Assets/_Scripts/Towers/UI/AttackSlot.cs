using System.Collections.Generic;
using TooltipSystem;
using UnityEngine;
using UnityEngine.UI;

public class AttackSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image[] _icons;

    private Dictionary<Image, TooltipTrigger> _tooltips = new();

    public void Setup(int index, Sprite icon, string attackName, string attackDescription)
    {
        _icons[index].sprite = icon;
        if (!_tooltips.TryGetValue(_icons[index], out var tooltip))
        {
            tooltip = _icons[index].GetComponent<TooltipTrigger>();
            _tooltips[_icons[index]] = tooltip;
        }

        tooltip.SetHeader(attackName);
        tooltip.SetContent(attackDescription);

        _icons[index].gameObject.SetActive(true);
    }

    public void Clear()
    {
        foreach (var icon in _icons)
            icon.gameObject.SetActive(false);
    }
}
