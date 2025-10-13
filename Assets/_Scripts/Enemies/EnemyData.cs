using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(menuName = "Enemy/New Enemy Data", fileName = "New Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public float Health = 1;
        public float Armor = 0;
        public float Speed = 4f;
        public int Damage = 1;
        public int DangerLevel = 1;
        public Sprite Sprite;
    }
}
