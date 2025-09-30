using MapGenerator.Settings;
using UnityEngine;

namespace MapGenerator.Saver
{
    public class MapSaver : MonoBehaviour
    {
        [SerializeField] private MapGenerator.Demo.MapGenerator _mapGenerator;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Save();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Load();
        }

        public void Save()
        {
            SaveSystem.SaveData(_mapGenerator.GetGenerationConfig(), "MapData");
        }

        public void Load()
        {
            GenerationConfig config = SaveSystem.LoadData<GenerationConfig>("MapData") as GenerationConfig;
            if (config == null)
            {
                Debug.LogWarning("No map saved");
                return;
            }

            _mapGenerator.SetGenerationConfig(config);
            _mapGenerator.GenerateMap();
        }
    }
}
