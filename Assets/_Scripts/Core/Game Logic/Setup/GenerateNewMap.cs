using System.Threading.Tasks;
using UnityEngine;

namespace Core.GameSetup
{
    public class GenerateNewMap : SetupHandler
    {
        [SerializeField] private MapGenerator.Demo.MapGenerator _mapGenerator;

        public override async Task Setup()
        {
            _mapGenerator.RandomizeConfig();
            await _mapGenerator.GenerateMapAsync();
        }
    }
}
