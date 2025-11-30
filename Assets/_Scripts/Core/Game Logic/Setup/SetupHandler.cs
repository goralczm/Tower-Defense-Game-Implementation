using System.Threading.Tasks;
using UnityEngine;

namespace Core.GameSetup
{
    public abstract class SetupHandler : MonoBehaviour
    {
        public abstract Task Setup();
    }
}
