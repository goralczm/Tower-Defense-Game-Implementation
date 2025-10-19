using MapGenerator.Settings;
using UnityEngine;

namespace MapGenerator.Saver
{
    public static class MapSaveSystem
    {
        public static void Save(MapGenerator.Demo.MapGenerator mapGenerator)
        {
            SaveSystem.SaveData(mapGenerator.GetGenerationConfig(), "MapData");
        }

        public static void Load(MapGenerator.Demo.MapGenerator mapGenerator)
        {
            GenerationConfig config = SaveSystem.LoadData<GenerationConfig>("MapData") as GenerationConfig;
            if (config == null)
            {
                Debug.LogWarning("No map saved");
                return;
            }

            mapGenerator.SetGenerationConfig(config);
            mapGenerator.GenerateMapAsync();
        }
    }
}
