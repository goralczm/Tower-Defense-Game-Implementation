using UnityEngine;

namespace Inventory
{
    public class PickupAnimationSpeedByLeftHideTimer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _anim;
        [SerializeField] private Pickup _pickup;

        private void Update()
        {
            float speed = Mathf.InverseLerp(_pickup.HideTimer, 0f, _pickup.TimeLeft) + 1f;
            _anim.SetFloat("Speed", speed);
        }
    }
}
