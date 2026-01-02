using ArtificeToolkit.Attributes;
using Attributes;
using UnityEngine;

namespace Towers
{
    [System.Serializable]
    public class TowerLevel
    {
        public Sprite Icon;
        public Color Color;
        public int Cost;
        public BaseAttributes<TowerAttributes> BaseAttributes;
        public AttackStrategy[] AttackStrategies;
    }

    [CreateAssetMenu(menuName = "Towers/New Tower Data", fileName = "New Tower Data")]
    public class TowerData : ScriptableObject
    {
        public int Id;
        public string Description;
        [ForceArtifice] public TowerLevel[] Levels;

        public int BuildCost => Levels[0].Cost;

        private void OnValidate()
        {
            int l = 0;
            foreach (var level in Levels)
            {
                foreach (var attack in level.AttackStrategies)
                    attack?.Validate();

                if (level.BaseAttributes.GetAttribute(TowerAttributes.InventoryCapacity) > level.AttackStrategies.Length)
                {
                    Debug.LogError($"[{name}:{l}] has Inventory Capacity greater than number of Attack Strategies.");
                }
                l++;
            }
        }

        public string GetBaseLevelAttributesDescription()
        {
            return new Attributes<TowerAttributes>(new(), Levels[0].BaseAttributes).GetAttributesDescription();
        }
    }
}
