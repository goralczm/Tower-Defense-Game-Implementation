using Attributes;
using UnityEngine;

namespace Core.Player
{
    public class TestPlayerAttributes : MonoBehaviour
    {
        [SerializeField] private Player _player;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                _player.Attributes.Mediator.AddModifier(new MathAttributeModifier<PlayerAttributes>(PlayerAttributes.Health, 0f, MathOperation.Add, 1));

            if (Input.GetKeyDown(KeyCode.O))
                _player.Attributes.Mediator.AddModifier(new MathAttributeModifier<PlayerAttributes>(PlayerAttributes.Health, 2f, MathOperation.Add, 1));

            if (Input.GetKeyDown(KeyCode.P))
                Debug.Log($"Player Health: {_player.Attributes.GetAttribute(PlayerAttributes.Health)}");
        }
    }
}
