using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class PunchOnEnable : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _slightlyRandomize;

        private void OnEnable()
        {
            transform.DOComplete();

            float punch = .25f;
            float duration = .2f;
            int vibrato = 5;
            float elasticity = .4f;

            if (_slightlyRandomize)
            {
                punch = Random.Range(.2f, .3f);
                duration = Random.Range(.15f, .25f);
                vibrato = Random.Range(4, 6);
                elasticity = Random.Range(.35f, .45f);
            }

            transform.DOPunchScale(Vector3.one * punch, duration, vibrato, elasticity);
        }
    }
}
