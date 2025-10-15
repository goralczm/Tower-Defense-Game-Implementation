using UnityEngine;

namespace Towers
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        protected Transform _target;
        protected float _damage;

        public void Setup(Transform target, float damage)
        {
            _target = target;
            _damage = damage;
        }
    }
}
