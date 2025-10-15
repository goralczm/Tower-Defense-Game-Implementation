using Attributes;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(menuName = "Enemy/New Enemy Data", fileName = "New Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public BaseAttributes<EnemyAttributes> BaseAttributes;
        public int DangerLevel = 1;
        public Sprite Sprite;
    }
}
