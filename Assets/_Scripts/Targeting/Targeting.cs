using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using Core;
using Core.Cache;

namespace Targeting
{
    [Serializable]
    public enum TargetingOptions
    {
        First,
        Last,
        Strongest,
    }

    public static class Targeting
    {
        public static LayerMask ObstaclesLayer = 1 << 9;

        public delegate float SortingCondition(ITargetable target);

        public static List<ITargetable> GetNTargetsInRangeByConditions(Vector2 origin, float range, int targetsCount, TargetingOptions targetingOption, List<Alignment> targetAlignments = null, bool needClearVision = false, Transform originTransform = null)
        {
            List<ITargetable> targetsInRange = GetTargetsInRange(origin, range, targetAlignments, originTransform);

            return GetNTargetsByCondition(origin, targetsInRange, targetsCount, targetingOption, needClearVision);
        }

        private static List<ITargetable> GetNTargetsByCondition(Vector2 origin, List<ITargetable> targets, int targetsCount, TargetingOptions targetingOption, bool needClearVision = false)
        {
            List<ITargetable> targetsCopy = new List<ITargetable>(targets);
            List<ITargetable> selectedTargets = new List<ITargetable>();

            for (int i = 0; i < targetsCount; i++)
            {
                ITargetable bestEnemy = GetEnemyByCondition(origin, targetsCopy, GetSortingCondition(origin, targetingOption), needClearVision);
                if (bestEnemy == null) break;

                selectedTargets.Add(bestEnemy);
                targetsCopy.Remove(bestEnemy);
            }

            return selectedTargets;
        }

        private static ITargetable GetEnemyByCondition(Vector2 origin, List<ITargetable> targets, SortingCondition sortingCondition, bool needClearVision = false)
        {
            if (targets.Count == 0)
                return null;

            ITargetable bestCandidate = null;
            float bestCondition = float.MinValue;
            int bestPriority = int.MinValue;

            foreach (var target in targets)
            {
                if (needClearVision && !HasClearVision(origin, target.Transform.position, target.Transform))
                    continue;

                int priority = target.TargetingPriority;
                float condition = sortingCondition(target);

                if (bestCandidate == null || priority > bestPriority || (priority == bestPriority && condition > bestCondition))
                {
                    bestCandidate = target;
                    bestPriority = priority;
                    bestCondition = condition;
                }
            }

            if (needClearVision && bestCandidate != null && !HasClearVision(origin, bestCandidate.Transform.position, bestCandidate.Transform))
                return null;

            return bestCandidate;
        }

        private static bool HasClearVision(Vector2 origin, Vector2 target, Transform self)
        {
            return !Physics2D.LinecastAll(origin, target)
                .Any(h =>
                    h.transform != self &&
                    Helpers.IsInLayerMask(h.collider.gameObject.layer, ObstaclesLayer)
                );
        }

        private static SortingCondition GetSortingCondition(Vector2 origin, TargetingOptions targetingOption)
        {
            switch (targetingOption)
            {
                case TargetingOptions.Strongest:
                    return target => target.Strength;
                case TargetingOptions.First:
                    return target => target.GetDistance(origin);
                case TargetingOptions.Last:
                    return target => -target.GetDistance(origin);
            }

            return target => target.GetDistance(origin);
        }

        public static List<ITargetable> GetTargetsInRange(Vector2 origin, float range, List<Alignment> targetAlignments = null, Transform originTransform = null)
        {
            List<ITargetable> targetsInRange = new List<ITargetable>();
            List<ITargetable> standByTargets = new List<ITargetable>();

            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range);
            foreach (Collider2D hit in hits)
            {
                if (hit.transform == originTransform || !hit.gameObject.activeSelf)
                    continue;

                if (TargetsCache.TryGetTarget(hit, out ITargetable target))
                {
                    if (targetAlignments != null && !targetAlignments.Contains(target.Alignment))
                        continue;

                    targetsInRange.Add(target);
                }
            }

            if (targetsInRange.Count == 0 && hits.Length > 0)
                targetsInRange.AddRange(standByTargets);

            return targetsInRange;
        }

        public static List<Vector2> GetPathPointsInRange(Vector2 position, float range, int points, Grid grid = null)
        {
            List<Vector2> validPositions = new List<Vector2>();

            float angleBetweenPoints = 2f * Mathf.PI / points;
            for (int i = 0; i < points; i++)
            {
                Vector3 dir = new Vector2(Mathf.Cos(i * angleBetweenPoints), Mathf.Sin(i * angleBetweenPoints));

                RaycastHit2D[] hits = Physics2D.RaycastAll(position, dir, range);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null && hit.transform.name.Contains("Path"))
                    {
                        Vector2 hitPoint = hit.point;
                        Vector2 hitDir = hitPoint - position;

                        float dist = Vector2.Distance(position, hitPoint);
                        hitDir.Normalize();
                        hitDir *= dist + .5f;

                        Vector2 formattedPos = position + hitDir;
                        if (grid != null)
                            formattedPos = grid.GetCellCenterLocal(grid.WorldToCell(formattedPos));
                        validPositions.Add(formattedPos);
                    }
                }
            }

            return validPositions;
        }
    }
}
