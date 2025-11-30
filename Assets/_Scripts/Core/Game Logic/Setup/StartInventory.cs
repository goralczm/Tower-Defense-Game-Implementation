using Core.GameSetup;
using System.Collections.Generic;
using System.Threading.Tasks;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class StartInventory : SetupHandler
    {
        public List<ProjectileData> StartItems;
        public Inventory.Inventory Inventory;

        public override async Task Setup()
        {
            foreach (var item in StartItems)
                Inventory.Add(item);
        }
    }
}
