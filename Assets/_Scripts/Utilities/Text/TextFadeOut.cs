using TMPro;
using UnityEngine;

namespace Utilities.Text
{
    public class TextFadeOut : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 10f)] private float _swirlFreq = 0f;
        [SerializeField, Range(0f, 5f)] private float _swirlAmpl = 0f;
        [SerializeField, Range(0f, 1f)] private float _moveSpeed = 1f;
        [SerializeField, Range(0f, 1f)] private float _opacitySpeed = 1f;

        [Header("References")]
        [SerializeField] private TextMeshPro _text;

        private float _time;

        private void Awake()
        {
            _time = Random.value;
        }

        private void OnEnable()
        {
            _text.color = _text.color.SetOpacity(1f);
        }

        private void Update()
        {
            _time += Time.deltaTime;

            float swirl = Mathf.Sin(_time * _swirlFreq) * _swirlAmpl * Time.deltaTime;

            transform.position = new Vector2(transform.position.x + swirl, transform.position.y + _moveSpeed * Time.deltaTime);
            _text.color = _text.color.SetOpacity(_text.color.a - _opacitySpeed * Time.deltaTime);
            if (_text.color.a <= .01f)
                gameObject.SetActive(false);
        }
    }
}
