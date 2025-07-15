using UnityEngine;

[CreateAssetMenu(menuName = "Path Generation/Path Preset", fileName = "New Path Preset")]
public class PathPreset : ScriptableObject
{
    public PathSettings PathSettings;
    public MazeGenerationSettings MazeGenerationSettings;
    public EnvironmentSettings EnvironmentSettings;
}
