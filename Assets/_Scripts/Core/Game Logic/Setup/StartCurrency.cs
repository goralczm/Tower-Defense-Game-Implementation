using UnityEngine;
using Currency;
using System.Threading.Tasks;

namespace Core.GameSetup
{
    public class StartCurrency : SetupHandler
    {
        [Header("References")]
        [SerializeField] private Bank _bank;

        public override async Task Setup()
        {
            _bank.SetCurrency(_bank.StartCurrency);
        }
    }
}
