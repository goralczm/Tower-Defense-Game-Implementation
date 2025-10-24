using Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace Waves
{
    [System.Serializable]
    public class WaveEntry
    {
        public EnemyData Enemy;
        public AnimationCurve Interval;
        public AnimationCurve Count;

        public float GetIntervalByWave(int wave) => Interval.Evaluate(wave);
        public int GetCountByWave(int wave) => Mathf.RoundToInt(Count.Evaluate(wave));
    }

    [CreateAssetMenu(menuName = "Waves/New Waves Data", fileName = "New Waves Data")]
    public class WavesData : ScriptableObject
    {
        public List<WaveEntry> Entries = new();
        public int Reward;
        public int WavesCount;
    }
}
