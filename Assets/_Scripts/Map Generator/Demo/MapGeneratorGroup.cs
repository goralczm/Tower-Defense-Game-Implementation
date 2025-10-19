using UnityEngine;

namespace MapGenerator.Demo
{
    public class MapGeneratorGroup : MonoBehaviour
    {
        [SerializeField] private MapGenerator[] _mapGenerators;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GenerateCombinedMap();
            }
        }

        private void GenerateCombinedMap()
        {
            for (int i = 0; i < _mapGenerators.Length; i++)
                _mapGenerators[i].GenerateMapAsync();
        }
    }
}
