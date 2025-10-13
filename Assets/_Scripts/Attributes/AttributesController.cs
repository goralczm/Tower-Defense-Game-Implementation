using UnityEngine;

namespace Attributes
{
    public class AttributesController : MonoBehaviour
    {
        [SerializeField] private BaseAttributes _baseStats;

        [SerializeField] private Attributes _stats;

        private void Awake()
        {
            _stats = new(new(), _baseStats);
        }

        private void Update()
        {
            _stats.Mediator.Update(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                var modifier = new BasicStatModifier(AttributeType.Health, 3f, v => v + 2);

                _stats.Mediator.AddModifier(modifier);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                print(_stats.GetAttribute(AttributeType.Health));
            }
        }
    }
}