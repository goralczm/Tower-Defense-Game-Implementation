using TooltipSystem;
using UnityEngine;
using UnityEngine.UI;

public class AttackSlot : TooltipTrigger
{
    [Header("References")]
    [SerializeField] private Image _icon;

    private string _attackName;
    private string _attackDescription;

    public override string Header => _attackName;
    public override string Content => _attackDescription;

    public void Setup(Sprite icon, string attackName, string attackDescription)
    {
        _icon.sprite = icon;
        _attackName = attackName;
        _attackDescription = attackDescription;
    }
}
