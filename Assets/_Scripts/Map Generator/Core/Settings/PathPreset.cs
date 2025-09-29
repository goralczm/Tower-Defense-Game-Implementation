using MapGeneration;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Path Generation/Path Preset", fileName = "New Path Preset")]
public class PathPreset : ScriptableObject
{
    public PathSettings PathSettings;
    public MazeGenerationSettings MazeGenerationSettings;
    public EnvironmentSettings EnvironmentSettings;

    [SerializeField, SerializeReference] private List<IGenerator> Generators = new();
}
