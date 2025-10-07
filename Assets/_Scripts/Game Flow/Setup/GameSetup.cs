using Enemies;
using MapGenerator.Saver;
using UnityEngine;

namespace GameFlow.Setup
{
    public class GameSetup : MonoBehaviour
    {
        [SerializeField] private MapGenerator.Demo.MapGenerator _mapGenerator;
        [SerializeField] private EnemySpawner _enemySpawner;

        private void OnEnable()
        {
            _mapGenerator.OnMapGenerated += OnMapGenerated;
        }

        private void OnDisable()
        {
            _mapGenerator.OnMapGenerated -= OnMapGenerated;
        }

        private void OnMapGenerated(object sender, MapGenerator.Demo.MapGenerator.OnMapGeneratedEventArgs e)
        {
            _enemySpawner.transform.position = e.StartPoint;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    _mapGenerator.RandomizeConfig();

                _mapGenerator.GenerateMap();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_enemySpawner.IsStopped)
                    _enemySpawner.StartSpawner();
                else
                    _enemySpawner.StopSpawner();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                Save();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Load();
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
