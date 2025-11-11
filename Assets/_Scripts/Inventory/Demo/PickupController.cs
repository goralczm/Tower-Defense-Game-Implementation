using Core;
using UnityEngine;

namespace Inventory.Demo
{
    public class PickupController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _pickupRange = .2f;
        [SerializeField] private LayerMask _pickupsLayer;

        [Header("References")]
        [SerializeField] private Inventory _inventory;

        private void Update()
        {
            var hits = Physics2D.OverlapCircleAll(MouseInput.GetMouseWorldPosition(), _pickupRange, _pickupsLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Pickup>(out var pickup) && _inventory.Add(pickup.Item))
                    pickup.gameObject.SetActive(false);
            }
        }
    }
}
