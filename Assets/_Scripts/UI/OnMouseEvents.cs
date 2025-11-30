using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class OnMouseEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent OnPointerEnterEvents;
        public UnityEvent OnPointerExitEvents;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvents?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvents?.Invoke();
        }
    }
}
