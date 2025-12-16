using Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace Waves
{
    [System.Serializable]
    public class WaveEntry
    {
        public EnemyData Enemy;
        public AnimationCurve SpawnRate;
        public AnimationCurve SpawnInterval;

        public int GetCountByWave(int wave) => (int)(SpawnRate.Evaluate(wave * .1f) * 100f);
        public float GetIntervalByWave(int wave) => SpawnInterval.Evaluate(wave * .1f);
    }

    [CreateAssetMenu(menuName = "Waves/New Waves Data", fileName = "New Waves Data")]
    public class WavesData : ScriptableObject
    {
        public List<WaveEntry> Entries = new();
        public int Reward;
        public int WavesCount;
    }
}
