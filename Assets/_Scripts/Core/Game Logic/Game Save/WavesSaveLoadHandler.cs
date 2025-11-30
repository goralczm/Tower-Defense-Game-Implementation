using System.Threading.Tasks;
using UnityEngine;
using Waves;

namespace Core.GameSave
{
    public class WavesSaveLoadHandler : SaveLoadHandler
    {
        [Header("References")]
        [SerializeField] private WavesController _wavesController;

        public override async Task Load()
        {
            int? savedWaves = SaveSystem.SaveSystem.LoadData<int?>(SaveKey) as int?;

            if (savedWaves == null)
                await FallbackSetup?.Setup();
            else
                _wavesController.SetCurrentWave(savedWaves.Value);
        }

        public override void Save()
        {
            int currentWave = _wavesController.CurrentWave;

            SaveSystem.SaveSystem.SaveData(currentWave, SaveKey);
        }
    }
}
