using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(menuName = "Enemy/New Enemy Data", fileName = "New Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public float Health;
        public float Armor;
        public float Speed;
        public int Damage;
        public Sprite Sprite;
    }
}
