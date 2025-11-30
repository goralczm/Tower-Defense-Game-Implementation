using System.Threading.Tasks;
using UnityEngine;

namespace Core.GameSetup
{
    public class StartPlayerAttirbutes : SetupHandler
    {
        [SerializeField] private Player.Player _player;

        public override async Task Setup()
        {
            _player.SetAttributes(new(new(), _player.BaseAttributes));
        }
    }
}