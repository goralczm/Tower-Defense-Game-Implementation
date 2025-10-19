using UnityEngine;

namespace MapGenerator.Settings
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Path Generation/Path Settings", fileName = "New Path Settings")]
    public class PathSettings : ScriptableObject
    {
        [Header("Path Settings")]
        public bool EnforceMinimalPathLength = false;
        public float MinimalPathLength = 0;
        public bool EnforceMaximumStraightTilesInRow = false;
        public int MaximumStraightTilesInRow = 5;
        //TODO: MaximalPathLength

        [Header("Roundabouts")]
        public bool EnforceRoundabouts = true;
        [Range(2, 10)] public int SmallestRoundaboutSize = 2;
        [Range(2, 10)] public int BiggestRoundaboutSize = 4;
        public bool RandomizeRoundaboutSize = true;
        [Range(0f, 1f)] public float RoundaboutProbability = .2f;
        public int MinimalTilesDistanceBetweenRoundabouts = 2;
        [Range(0, 2)] public int MinimalRounabouts = 0;

        [Header("Generation")]
        public int Steps = 10000;
        public int MaximumGenerationDepth = 30;
        public bool ExperimentalRandomizeSeedWhenGenerationDepthExceeded;

        private void OnValidate()
        {
            MaximumGenerationDepth = Mathf.Min(MaximumGenerationDepth, 2000);
        }
    }
}
