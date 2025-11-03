using Inventory;
using System.Collections.Generic;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class StartInventory : MonoBehaviour
    {
        public List<ProjectileData> StartItems;
        public Inventory.Inventory Inventory;

        private void Start()
        {
            foreach (var item in StartItems)
                Inventory.Add(item);
        }
    }
}
