using UnityEngine;

namespace ObjectPooling
{
    public class PoolObject : MonoBehaviour
    {
        private string _tag;

        private void OnDisable()
        {
            Release();
        }

        public void Init(string tag)
        {
            _tag = tag;
        }

        protected void Release()
        {
            PoolManager.Instance?.ReleaseObject(_tag, gameObject);
        }
    }
}
