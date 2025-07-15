using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathRendererAddons : MonoBehaviour
{
    [SerializeField] private GameObject _generationText;
    
    private AudioSystem.AudioSystem _audio;
    private CameraShake _shake;

    private void OnEnable()
    {
        _audio = FindFirstObjectByType<AudioSystem.AudioSystem>();
        _shake = FindFirstObjectByType<CameraShake>();
        PathRenderer.OnTileChanged += HandleTileChanged;
        PathGenerationOrchestrator.OnPathGenerationStarted += ShowGenerationText;
        PathGenerationOrchestrator.OnPathGenerationEnded += HideGenerationText;
    }

    private void OnDisable()
    {
        PathRenderer.OnTileChanged -= HandleTileChanged;
        PathGenerationOrchestrator.OnPathGenerationStarted -= ShowGenerationText;
        PathGenerationOrchestrator.OnPathGenerationEnded -= HideGenerationText;
    }
    
    private void ShowGenerationText(object sender, EventArgs args)
    {
        _generationText.SetActive(true);
    }
    
    private void HideGenerationText(object sender, PathGenerationOrchestrator.OnPathGeneratedEventArgs args)
    {
        _generationText.SetActive(false);
    }

    private void HandleTileChanged(TileBase tileBase)
    {
        if (tileBase == null) _audio.PlaySoundFromGroup("Sounds", "tile destroyed");
        else if (tileBase.name.ToLower().Contains("corner")) _audio.PlaySoundFromGroup("Sounds", "corner placed");
        else _audio.PlaySoundFromGroup("Sounds", "tile placed");

        _shake.DefaultShake();
    }
}
