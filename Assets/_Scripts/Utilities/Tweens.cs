using DG.Tweening;
using System;
using UnityEngine;

namespace Utilities
{
    public static class Tweens
    {
        public static void FlyTowards(this Transform origin, Vector2 target, Ease ease, float speed, Action onCompleteAction)
        {
            origin.DOComplete();
            origin
                .DOMove(target, speed)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    onCompleteAction?.Invoke();
                });
        }

        public static void SimplePunch(this Transform target)
        {
            target.DOComplete();
            target
                .DOPunchScale(Vector3.one * 0.25f, 0.2f, 5, .4f);
        }
    }
}
