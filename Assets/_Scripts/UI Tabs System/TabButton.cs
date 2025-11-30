using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private GameObject _connectedTab;
    [SerializeField] private Image _background;

    private TabGroup _tabGroup;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    public void ActivePage()
    {
        _connectedTab.SetActive(true);
    }

    public void DeactivePage()
    {
        _connectedTab.SetActive(false);
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        _background.sprite = sprite;
    }

    public void SetBackgroundColor(Color color)
    {
        _background.color = color;
    }

    public void SetTabGroup(TabGroup tabGroup)
    {
        _tabGroup = tabGroup;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tabGroup.OnTabExit(this);
    }
}
