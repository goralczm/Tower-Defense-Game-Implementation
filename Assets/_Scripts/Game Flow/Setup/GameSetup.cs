using Enemies;
using MapGenerator.Saver;
using UnityEngine;
using Waves;

namespace GameFlow.Setup
{
    public class GameSetup : MonoBehaviour
    {
        [SerializeField] private MapGenerator.Demo.MapGenerator _mapGenerator;
        [SerializeField] private WavesController _waveController;

        private void OnEnable()
        {
            _mapGenerator.OnMapGenerationEnded += OnMapGenerated;
        }

        private void OnDisable()
        {
            _mapGenerator.OnMapGenerationEnded -= OnMapGenerated;
        }

        private void OnMapGenerated(object sender, MapGenerator.Demo.MapGenerator.OnMapGeneratedEventArgs e)
        {
            _waveController.transform.position = e.StartPoint;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    _mapGenerator.RandomizeConfig();

                _mapGenerator.GenerateMapAsync();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                _mapGenerator.CancelBuild();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _waveController.StartGenerator();
            }

            /*if (Input.GetKeyDown(KeyCode.Alpha1))
                Save();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Load();*/
        }

        public void Load()
        {
            MapSaveSystem.Load(_mapGenerator);
        }

        public void Save()
        {
            MapSaveSystem.Save(_mapGenerator);
        }
    }
}
