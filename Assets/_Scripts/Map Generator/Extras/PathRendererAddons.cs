using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathRendererAddons : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _delayBetweenTilesInMs = 100;
    [SerializeField] private int _minDelay = 5;
    [SerializeField] private int _speedUpFactor = 1;
    
    [Header("References")]
    [SerializeField] private MapGenerator.Core.Map.MapGenerator _mapGenerator;
    [SerializeField] private GameObject _generationText;
    
    private AudioSystem.AudioSystem _audio;
    private CameraShake _shake;

    private int _speedUp;
    
    private void OnEnable()
    {
        _audio = FindFirstObjectByType<AudioSystem.AudioSystem>();
        _shake = FindFirstObjectByType<CameraShake>();
        PathRenderer.OnTileChangedAsync += HandleTileChanged;
        EnvironmentGenerator.OnObstaclePlacedAsync += HandleObstaclePlaced;
        EnvironmentGenerator.OnObstacleDestroyedAsync += HandleObstacleDestroyed;
        _mapGenerator.OnMapGenerationStarted += MapGenerationStarted;
        _mapGenerator.OnMapGenerationEnded += MapGenerationEnded;
    }

    private void OnDisable()
    {
        PathRenderer.OnTileChangedAsync -= HandleTileChanged;
        EnvironmentGenerator.OnObstaclePlacedAsync -= HandleObstaclePlaced;
        EnvironmentGenerator.OnObstacleDestroyedAsync -= HandleObstacleDestroyed;
        _mapGenerator.OnMapGenerationStarted -= MapGenerationStarted;
        _mapGenerator.OnMapGenerationEnded -= MapGenerationEnded;
    }
    
    private void MapGenerationStarted()
    {
        _speedUp = 0;
        _generationText.SetActive(true);
    }
    
    private void MapGenerationEnded()
    {
        _generationText.SetActive(false);
    }

    private async Task HandleTileChanged(TileBase tileBase)
    {
        if (tileBase == null) _audio.PlaySoundFromGroup("Sounds", "tile destroyed");
        else if (tileBase.name.ToLower().Contains("corner")) _audio.PlaySoundFromGroup("Sounds", "corner placed");
        else _audio.PlaySoundFromGroup("Sounds", "tile placed");

        _shake.DefaultShake();
        await Delay();
    }
    
    private async Task HandleObstaclePlaced(GameObject obstacle)
    {
        _audio.PlaySoundFromGroup("Sounds", "tile placed");

        _shake.DefaultShake();
        await Delay();
    }

    private async Task HandleObstacleDestroyed(Vector2 position)
    {
        _audio.PlaySoundFromGroup("Sounds", "tile placed");
        
        _shake.DefaultShake();
        await Delay();
    }

    private async Task Delay()
    {
        int delay = Mathf.Max(_minDelay, _delayBetweenTilesInMs - _speedUp);
        await Task.Delay(delay);
        _speedUp++;
    }
}
