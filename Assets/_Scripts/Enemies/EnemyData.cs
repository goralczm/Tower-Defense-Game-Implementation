using Attributes;
using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(menuName = "Enemy/New Enemy Data", fileName = "New Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public BaseAttributes<EnemyAttributes> BaseAttributes;
        public int Reward;
        public int DangerLevel = 1;
        public Sprite Sprite;
        public List<EnemyData> Children;
        public DamageType[] Resistances = new DamageType[0];
        public DamageType[] Vulnerabilities = new DamageType[0];
    }
}
