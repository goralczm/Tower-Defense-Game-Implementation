using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PresetSelectorUI : MonoBehaviour
{
    [SerializeField] private PathGenerationOrchestrator _orchestrator;
    
    [SerializeField] private PathPreset[] _presets;
    [SerializeField] private TMP_Dropdown _presetsDropdown;
    
    private void Awake()
    {
        _presetsDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new();
        foreach (var preset in _presets)
        {
            options.Add(new(preset.name));
        }

        _presetsDropdown.AddOptions(options);
        _presetsDropdown.RefreshShownValue();
    }
    
    public void SetPresetByIndex(int index)
    {
        _orchestrator.SetPathPreset(_presets[index]);
    }
}
