using UnityEngine;

[CreateAssetMenu(menuName = "Path Generation/Path Settings", fileName = "New Path Settings")]
public class PathSettings : ScriptableObject
{
    [Header("Path Settings")]
    public int Steps = 10000;
    
    public bool MoveRootToEnd = true;
    public bool EnforceMinimalPathLength = false;
    public float MinimalPathLength = 0;
    public bool EnforceMaximumStraightTilesInRow = false;
    public int MaximumStraightTilesInRow = 5;
    public bool EnforceRoundabouts = true;
    public int RoundaboutPercentageChance = 20;
    public int MinimalTilesDistanceBetweenRoundabouts = 2;
    public int MaximumGenerationDepth = 30;
    public bool ExperimentalRandomizeSeedWhenGenerationDepthExceeded;
    
    public void SetRandomSteps()
    {
        Steps = Random.Range(10000, 20000);
    }

    private void OnValidate()
    {
        MinimalPathLength = Mathf.Min(MinimalPathLength, 60);
        MaximumGenerationDepth = Mathf.Min(MaximumGenerationDepth, 2000);
    }
}
