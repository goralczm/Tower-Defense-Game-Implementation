using System.Threading.Tasks;
using UnityEngine;
using Waves;

namespace Core.GameSetup
{
    public class StartWave : SetupHandler
    {
        [Header("References")]
        [SerializeField] private WavesController _wavesController;

        public override async Task Setup()
        {
            _wavesController.SetCurrentWave(_wavesController.StartWave);
        }
    }
}
