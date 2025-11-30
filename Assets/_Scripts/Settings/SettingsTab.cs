using GameSettings.Inputs.Basic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameSettings
{
    public class SettingsTab : MonoBehaviour
    {
        private List<SettingsInput> _inputs = new();

        private void Awake()
        {
            _inputs = GetComponentsInChildren<SettingsInput>(true).ToList();
        }

        public void ResetToDefaults()
        {
            for (int i = 0; i < _inputs.Count; i++)
                _inputs[i].ResetToDefault();
        }
    }
}
