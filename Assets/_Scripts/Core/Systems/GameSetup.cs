using Core.GameSave;
using Core.GameSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Waves;

namespace Core.Systems
{
    public class GameSetup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapGenerator.Demo.MapGenerator _mapGenerator;
        [SerializeField] private WavesController _waveController;
        [SerializeField] private GameObject _setupNewGameObject;

        private List<SaveLoadHandler> _saveLoadHandlers;
        private List<SetupHandler> _setupHandlers;
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
            {
                _saveLoadHandlers = GetComponentsInChildren<SaveLoadHandler>(true).ToList();
                _saveLoadHandlers.Sort((h1, h2) => h2.Priority.CompareTo(h1.Priority));

                await Load();
            }
            else
            {
                _setupHandlers = GetComponentsInChildren<SetupHandler>(true).ToList();

                foreach (var handler in _setupHandlers)
                    await handler.Setup();
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
            if (Input.GetKeyDown(KeyCode.Space))
                _waveController.StartGenerator();
        }

        public async Task Load()
        {
            for (int i = 0; i < _saveLoadHandlers.Count; i++)
                await _saveLoadHandlers[i].Load();
        }

        [ContextMenu("Force Save")]
        public void Save()
        {
            for (int i = 0; i < _saveLoadHandlers.Count; i++)
                _saveLoadHandlers[i].Save();
        }
    }
}
