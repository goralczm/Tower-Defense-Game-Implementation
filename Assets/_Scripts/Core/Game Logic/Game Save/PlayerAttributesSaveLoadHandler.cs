using Attributes;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.GameSave
{
    public class PlayerAttributesSaveLoadHandler : SaveLoadHandler
    {
        [SerializeField] private Player.Player _player;

        public override async Task Load()
        {
            Attributes<PlayerAttributes> savedAttributes = SaveSystem.SaveSystem.LoadData<Attributes<PlayerAttributes>>(SaveKey) as Attributes<PlayerAttributes>;

            if (savedAttributes == null)
                await FallbackSetup?.Setup();
            else
            {
                savedAttributes.Mediator.ForceReconnectModifiers();
                _player.SetAttributes(savedAttributes);
            }
        }

        public override void Save()
        {
            Attributes<PlayerAttributes> playerAttributes = _player.Attributes;

            SaveSystem.SaveSystem.SaveData(playerAttributes, SaveKey);
        }
    }
}
