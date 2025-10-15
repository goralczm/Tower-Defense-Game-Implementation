using Enemies;
using System;
using System.Collections.Generic;
using UnityEngine;
using Paths;

namespace Targeting
{
    [Serializable]
    public enum TargetingOptions
    {
        First,
        Last,
        Strongest,
        Fastest,
    }

    public static class Targeting
    {
        public static LayerMask GroundLayer = 1 << 6;
        public static LayerMask TowersLayer = 1 << 7;
        public static LayerMask EnemiesLayer = 1 << 8;
        public static LayerMask ObstaclesLayer = 1 << 9;

        public delegate float SortingCondition(EnemyBehaviour enemy);

        public static List<EnemyBehaviour> GetNEnemiesInRangeByConditions(Vector2 origin, float range, int enemiesCount, TargetingOptions targetingOption)
        {
            List<EnemyBehaviour> enemiesInRange = GetEnemiesInRange(origin, range);
            return GetNEnemiesByCondition(enemiesInRange, enemiesCount, targetingOption);
        }

        private static List<EnemyBehaviour> GetNEnemiesByCondition(List<EnemyBehaviour> enemies, int enemiesCount, TargetingOptions targetingOption)
        {
            List<EnemyBehaviour> enemiesCopy = new List<EnemyBehaviour>(enemies);
            List<EnemyBehaviour> selectedEnemies = new List<EnemyBehaviour>();

            for (int i = 0; i < enemiesCount; i++)
            {
                EnemyBehaviour bestEnemy = GetEnemyByCondition(enemiesCopy, GetSortingCondition(targetingOption));
                if (bestEnemy == null)
                    break;

                selectedEnemies.Add(bestEnemy);
                enemiesCopy.Remove(bestEnemy);
            }

            return selectedEnemies;
        }

        private static EnemyBehaviour GetEnemyByCondition(List<EnemyBehaviour> enemies, SortingCondition sortingCondition)
        {
            if (enemies.Count == 0)
                return null;

            EnemyBehaviour bestCandidate = enemies[0];
            float highestCondition = sortingCondition(bestCandidate);

            for (int i = 1; i < enemies.Count; i++)
            {
                float currentCondition = sortingCondition(enemies[i]);
                if (currentCondition > highestCondition)
                {
                    bestCandidate = enemies[i];
                    highestCondition = currentCondition;
                }
            }

            return bestCandidate;
        }

        [Obsolete]
        public static List<EnemyBehaviour> GetSortedEnemiesInRange(Vector2 origin, float range, TargetingOptions targetingOption)
        {
            SortingCondition sortingCodition = enemy => enemy.PathTraveled;
            switch (targetingOption)
            {
                case TargetingOptions.Strongest:
                    sortingCodition = enemy => enemy.DangerLevel;
                    break;
                case TargetingOptions.Fastest:
                    sortingCodition = enemy => enemy.Attributes.GetAttribute(Attributes.EnemyAttributes.Speed);
                    break;
                case TargetingOptions.First:
                case TargetingOptions.Last:
                    sortingCodition = enemy => enemy.PathTraveled;
                    break;
            }

            List<EnemyBehaviour> enemiesInRange = GetEnemiesInRange(origin, range);
            List<EnemyBehaviour> sortedEnemies = SortEnemiesByCondition(enemiesInRange, sortingCodition);

            if (targetingOption != TargetingOptions.Last)
                sortedEnemies.Reverse();

            return sortedEnemies;
        }

        private static SortingCondition GetSortingCondition(TargetingOptions targetingOption)
        {
            switch (targetingOption)
            {
                case TargetingOptions.Strongest:
                    return enemy => enemy.DangerLevel;
                case TargetingOptions.First:
                    return enemy => enemy.PathTraveled;
                case TargetingOptions.Last:
                    return enemy => -enemy.PathTraveled;
            }

            return enemy => enemy.PathTraveled;
        }

        private static List<EnemyBehaviour> SortEnemiesByCondition(List<EnemyBehaviour> enemiesToSort, SortingCondition sortingCondition)
        {
            List<EnemyBehaviour> sortedEnemies = new List<EnemyBehaviour>(enemiesToSort);
            QuickSortEnemies(sortedEnemies, sortingCondition, 0, enemiesToSort.Count - 1);

            return sortedEnemies;
        }

        public static List<EnemyBehaviour> GetEnemiesInRange(Vector2 origin, float range)
        {
            List<EnemyBehaviour> enemiesInRange = new List<EnemyBehaviour>();
            List<EnemyBehaviour> standByEnemies = new List<EnemyBehaviour>();

            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, EnemiesLayer);
            foreach (Collider2D hit in hits)
            {
                if (!hit.gameObject.activeSelf)
                    continue;

                EnemyBehaviour enemy = EnemiesCache.GetEnemyByCollider(hit);

                // if (enemy.AvoidTargeting)
                //    continue;

                enemiesInRange.Add(enemy);
            }

            if (enemiesInRange.Count == 0 && hits.Length > 0)
                enemiesInRange.AddRange(standByEnemies);

            return enemiesInRange;
        }

        private static void QuickSortEnemies(List<EnemyBehaviour> enemies, SortingCondition sortingCondition, int left, int right)
        {
            if (left >= right)
                return;

            float pivot = MedianOfThree(enemies, sortingCondition, left, right);
            int index = Partition(enemies, sortingCondition, left, right, pivot);
            QuickSortEnemies(enemies, sortingCondition, left, index - 1);
            QuickSortEnemies(enemies, sortingCondition, index, right);
        }

        private static int Partition(List<EnemyBehaviour> enemies, SortingCondition sortingCondition, int left, int right, float pivot)
        {
            while (left <= right)
            {
                while (sortingCondition(enemies[left]) < pivot)
                    left++;
                while (sortingCondition(enemies[right]) > pivot)
                    right--;

                if (left <= right)
                {
                    SwapElements(enemies, left, right);
                    left++;
                    right--;
                }
            }

            return left;
        }

        private static float MedianOfThree(List<EnemyBehaviour> enemies, SortingCondition sortingCondition, int left, int right)
        {
            int mid = left + (right - left) / 2;

            if (sortingCondition(enemies[left]) > sortingCondition(enemies[mid]))
                SwapElements(enemies, left, mid);
            if (sortingCondition(enemies[left]) > sortingCondition(enemies[right]))
                SwapElements(enemies, left, right);
            if (sortingCondition(enemies[mid]) > sortingCondition(enemies[right]))
                SwapElements(enemies, mid, right);

            return sortingCondition(enemies[mid]);
        }

        private static void SwapElements(List<EnemyBehaviour> enemies, int firstIndex, int secondIndex)
        {
            EnemyBehaviour enemyCopy = enemies[firstIndex];
            enemies[firstIndex] = enemies[secondIndex];
            enemies[secondIndex] = enemyCopy;
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
