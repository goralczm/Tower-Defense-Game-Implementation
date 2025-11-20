using Inventory;
using UnityEngine;

namespace ObjectPooling
{
    public static class PickupFactory
    {
        public static Pickup CreatePickup(IItem item, Vector2 position)
        {
            var pickupObject = PoolManager.Instance.SpawnFromPool("Pickup", position, Quaternion.identity);
            var pickup = pickupObject.GetComponent<Pickup>();

            pickup.Setup(item);

            return pickup;
        }
    }
}
