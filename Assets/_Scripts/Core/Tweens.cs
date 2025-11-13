using DG.Tweening;
using System;
using UnityEngine;

namespace UI
{
    public class Tweens : MonoBehaviour
    {
        public static void FlyTowards(Transform origin, Vector2 target, Ease ease, float speed, Action onCompleteAction)
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

        public static void Punch(Transform target)
        {
            target.DOComplete();
            target
                .DOPunchScale(Vector3.one * 0.25f, 0.2f, 5, .4f);
        }
    }
}
