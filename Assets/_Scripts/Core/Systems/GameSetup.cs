using MapGenerator.Saver;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Waves;

namespace Core.Systems
{
    public class GameSetup : MonoBehaviour
    {
        [SerializeField] private MapGenerator.Demo.MapGenerator _mapGenerator;
        [SerializeField] private WavesController _waveController;

        private bool _setupDone;

        public event Action OnGameSetupDone;

        public bool SetupDone => _setupDone;

        private void OnEnable()
        {
            _mapGenerator.OnMapGenerationEnded += OnMapGenerated;
        }

        private void OnDisable()
        {
            _mapGenerator.OnMapGenerationEnded -= OnMapGenerated;
        }

        private async void Start()
        {
            if (GlobalSystems.Instance.LevelSettings.LoadGame)
                await Load();
            else
            {
                _mapGenerator.RandomizeConfig();
                await _mapGenerator.GenerateMapAsync();
            }

            _setupDone = true;
            OnGameSetupDone?.Invoke();
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
                _waveController.StartGenerator();

            /*if (Input.GetKeyDown(KeyCode.Alpha1))
                Save();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Load();*/
        }

        public async Task Load()
        {
            await MapSaveSystem.Load(_mapGenerator);
        }

        [ContextMenu("Force Save")]
        public void Save()
        {
            MapSaveSystem.Save(_mapGenerator);
        }
    }
}
