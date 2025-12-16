using UnityEngine;
using UnityEngine.UI;

namespace Core.Player
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Player _player;

        private void Update()
        {
            if (_player.Attributes != null)
                _healthSlider.value = _player.Attributes.GetAttribute(Attributes.PlayerAttributes.Health);
        }
    }
}
