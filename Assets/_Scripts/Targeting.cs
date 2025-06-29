using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum TargetingOptions
{
    First,
    Strongest,
    Last,
    Left,
    Right,
}

public static class Targeting
{
    public static LayerMask NodesLayers = 1 << 6;
    public static LayerMask TowersLayer = 1 << 7;
    public static LayerMask EnemiesLayer = 1 << 8;
    public static LayerMask ObstaclesLayer = 1 << 9;

    public delegate float SortingCondition(Enemy enemy);

    public static List<Enemy> GetNEnemiesInRangeByConditions(Vector2 origin, float range, int enemiesCount, TargetingOptions targetingOption, PathColor pathColorsFilter = 0)
    {
        List<Enemy> enemiesInRange = GetEnemiesInRange(origin, range, pathColorsFilter);
        return GetNEnemiesByCondition(enemiesInRange, enemiesCount, targetingOption);
    }

    public static List<Enemy> GetNEnemiesByCondition(List<Enemy> enemies, int enemiesCount, TargetingOptions targetingOption)
    {
        List<Enemy> enemiesCopy = new List<Enemy>(enemies);
        List<Enemy> selectedEnemies = new List<Enemy>();

        for (int i = 0; i < enemiesCount; i++)
        {
            Enemy bestEnemy = GetEnemyByCondition(enemiesCopy, GetSortingCondition(targetingOption));
            if (bestEnemy == null)
                break;

            selectedEnemies.Add(bestEnemy);
            enemiesCopy.Remove(bestEnemy);
        }

        return selectedEnemies;
    }

    private static Enemy GetEnemyByCondition(List<Enemy> enemies, SortingCondition sortingCondition)
    {
        if (enemies.Count == 0)
            return null;

        Enemy bestCandidate = enemies[0];
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
    public static List<Enemy> GetSortedEnemiesInRange(Vector2 origin, float range, TargetingOptions targetingOption, PathColor pathColorsFilter = 0)
    {
        SortingCondition sortingCodition = enemy => enemy.PathTraveled;
        switch (targetingOption)
        {
            case TargetingOptions.Strongest:
                sortingCodition = enemy => enemy.DifficultyLevel;
                break;
            case TargetingOptions.First:
            case TargetingOptions.Last:
                sortingCodition = enemy => enemy.PathTraveled;
                break;
        }

        List<Enemy> enemiesInRange = GetEnemiesInRange(origin, range, pathColorsFilter);
        List<Enemy> sortedEnemies = SortEnemiesByCondition(enemiesInRange, sortingCodition);

        if (targetingOption != TargetingOptions.Last)
            sortedEnemies.Reverse();

        return sortedEnemies;
    }

    private static SortingCondition GetSortingCondition(TargetingOptions targetingOption)
    {
        switch (targetingOption)
        {
            case TargetingOptions.Strongest:
                return enemy => enemy.DifficultyLevel;
            case TargetingOptions.First:
                return enemy => enemy.PathTraveled;
            case TargetingOptions.Last:
                return enemy => -enemy.PathTraveled;
        }

        return enemy => enemy.PathTraveled;
    }
    
    private static List<Enemy> SortEnemiesByCondition(List<Enemy> enemiesToSort, SortingCondition sortingCondition)
    {
        List<Enemy> sortedEnemies = new List<Enemy>(enemiesToSort);
        QuickSortEnemies(sortedEnemies, sortingCondition, 0, enemiesToSort.Count - 1);

        return sortedEnemies;
    }

    public static List<Enemy> GetEnemiesInRange(Vector2 origin, float range, PathColor pathColorsFilter = 0)
    {
        List<Enemy> enemiesInRange = new List<Enemy>();
        List<Enemy> standByEnemies = new List<Enemy>();

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, EnemiesLayer);
        foreach (Collider2D hit in hits)
        {
            if (!hit.gameObject.activeSelf)
                continue;

            Enemy enemy = EnemiesCache.GetEnemyByCollider(hit);

            // if (enemy.AvoidTargeting)
            //    continue;

            if (pathColorsFilter != 0 && !pathColorsFilter.HasFlag(enemy.PathColor))
            {
                standByEnemies.Add(enemy);
                continue;
            }

            enemiesInRange.Add(enemy);
        }

        if (enemiesInRange.Count == 0 && hits.Length > 0)
            enemiesInRange.AddRange(standByEnemies);

        return enemiesInRange;
    }

    public static void QuickSortEnemies(List<Enemy> enemies, SortingCondition sortingCondition, int left, int right)
    {
        if (left >= right)
            return;

        float pivot = MedianOfThree(enemies, sortingCondition, left, right);
        int index = Partition(enemies, sortingCondition, left, right, pivot);
        QuickSortEnemies(enemies, sortingCondition, left, index - 1);
        QuickSortEnemies(enemies, sortingCondition, index, right);
    }

    public static int Partition(List<Enemy> enemies, SortingCondition sortingCondition, int left, int right, float pivot)
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

    public static float MedianOfThree(List<Enemy> enemies, SortingCondition sortingCondition, int left, int right)
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

    private static void SwapElements(List<Enemy> enemies, int firstIndex, int secondIndex)
    {
        Enemy enemyCopy = enemies[firstIndex];
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