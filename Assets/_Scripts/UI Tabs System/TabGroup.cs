using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite _tabSprite;
    [SerializeField] private Color _idleColor;
    [SerializeField] private Color _hoverColor;
    [SerializeField] private Color _activeColor;

    [Header("Settings")]
    [SerializeField] private bool _setupOnStart = true;

    private List<TabButton> _tabButtons = new List<TabButton>();
    private TabButton _selectedButton;

    private void Start()
    {
        if (_setupOnStart)
            Setup();
    }

    public void Setup()
    {
        _tabButtons = GetComponentsInChildren<TabButton>(true).ToList();

        foreach (TabButton button in _tabButtons)
        {
            button.SetTabGroup(this);
            button.SetBackgroundSprite(_tabSprite);
        }

        OnTabSelected(_tabButtons[0]);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (_selectedButton != button)
            button.SetBackgroundColor(_hoverColor);
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        _selectedButton = button;
        ResetTabs();
        button.SetBackgroundColor(_activeColor);
        ShowCurrentActivePage();
    }

    private void ResetTabs()
    {
        foreach (TabButton button in _tabButtons)
        {
            if (button == _selectedButton)
                continue;

            button.SetBackgroundColor(_idleColor);
        }
    }

    private void ShowCurrentActivePage()
    {
        foreach (TabButton btn in _tabButtons)
        {
            if (btn == _selectedButton)
                btn.ActivePage();
            else
                btn.DeactivePage();
        }
    }
}
