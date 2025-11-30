using Core.GameSetup;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace Core.GameSave
{
    public abstract class SaveLoadHandler : MonoBehaviour
    {
        public SetupHandler FallbackSetup;
        [SerializeField] private int _priority;
        [SerializeField] private string _saveKey;

        public int Priority => _priority;
        public string SaveKey => $"{Helpers.GetLevelName()}_{_saveKey}";

        public abstract void Save();
        public abstract Task Load();
    }
}
