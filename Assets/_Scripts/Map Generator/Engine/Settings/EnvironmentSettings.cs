using UnityEngine;

[CreateAssetMenu(menuName = "Path Generation/Environment Settings", fileName = "New Environment Settings")]
public class EnvironmentSettings : ScriptableObject
{
    [Range(0f, 1f)] public float NoiseThreshold = .5f;
    [Range(0f, 1f)] public float ObstacleNearPathProbability = .2f;
    public NoiseSettings NoiseSettings;
}
