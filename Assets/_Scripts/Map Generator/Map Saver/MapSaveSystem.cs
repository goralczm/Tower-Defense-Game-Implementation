using MapGenerator.Settings;
using System.Threading.Tasks;
using UnityEngine;

namespace MapGenerator.Saver
{
    public static class MapSaveSystem
    {
        public static void Save(MapGenerator.Demo.MapGenerator mapGenerator)
        {
            SaveSystem.SaveSystem.SaveData(mapGenerator.GetGenerationConfig(), "MapData");
        }

        public static async Task Load(MapGenerator.Demo.MapGenerator mapGenerator)
        {
            GenerationConfig config = SaveSystem.SaveSystem.LoadData<GenerationConfig>("MapData") as GenerationConfig;
            if (config == null)
            {
                Debug.LogWarning("No map saved");
                return;
            }

            mapGenerator.SetGenerationConfig(config);
            await mapGenerator.GenerateMapAsync();
        }
    }
}
