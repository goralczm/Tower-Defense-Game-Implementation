using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Systems
{
    public class UISoundPlayer : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            GlobalSystems.Instance.AudioSystem.PlaySoundFromGroup("Sounds", "click");
        }
    }
}
