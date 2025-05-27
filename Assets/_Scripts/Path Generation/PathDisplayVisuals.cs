using AudioSystem;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathDisplayVisuals : MonoBehaviour
{
    private PathDisplay _pathDisplay;
    private AudioSystem.AudioSystem _audio;
    private CameraShake _shake;

    private void OnEnable()
    {
        _pathDisplay = GetComponent<PathDisplay>();
        _audio = FindFirstObjectByType<AudioSystem.AudioSystem>();
        _shake = FindFirstObjectByType<CameraShake>();
        _pathDisplay.OnTileChanged += HandleTileChanged;
    }

    private void OnDisable()
    {
        _pathDisplay.OnTileChanged -= HandleTileChanged;
    }

    private void HandleTileChanged(TileBase tileBase)
    {
        if (tileBase == null) _audio.PlaySoundFromGroup("Sounds", "tile destroyed");
        else if (tileBase.name.ToLower().Contains("corner")) _audio.PlaySoundFromGroup("Sounds", "corner placed");
        else _audio.PlaySoundFromGroup("Sounds", "tile placed");

        _shake.DefaultShake();
    }
}
