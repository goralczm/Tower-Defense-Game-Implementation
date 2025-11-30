using MapGenerator.Settings;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.GameSave
{
    public class MapSaveLoadHandler : SaveLoadHandler
    {
        [Header("References")]
        [SerializeField] private MapGenerator.Demo.MapGenerator _mapGenerator;

        public override async Task Load()
        {
            GenerationConfig config = SaveSystem.SaveSystem.LoadData<GenerationConfig>(SaveKey) as GenerationConfig;
            if (config == null)
                await FallbackSetup?.Setup();
            else
            {
                _mapGenerator.SetGenerationConfig(config);
                await _mapGenerator.GenerateMapAsync();
            }
        }

        public override void Save()
        {
            SaveSystem.SaveSystem.SaveData(_mapGenerator.GetGenerationConfig(), SaveKey);
        }
    }
}
