using System.Collections.Generic;
using UnityEngine;

public class PerlinNoisePathGenerator : MonoBehaviour
{
    public int pathLength = 200;
    public float stepSize = 0.5f;
    public float noiseScale = 0.1f;
    public Vector2 startPosition = new Vector2(0, 0);

    private List<Vector2> pathPoints = new List<Vector2>();

    void Start()
    {
        GeneratePath();
        DrawPath();
    }

    void GeneratePath()
    {
        pathPoints.Clear();
        Vector2 currentPos = startPosition;

        for (int i = 0; i < pathLength; i++)
        {
            float angle = Mathf.PerlinNoise(i * noiseScale, 0) * Mathf.PI * 2f;

            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            currentPos += direction * stepSize;

            pathPoints.Add(currentPos);
        }
    }

    void DrawPath()
    {
        for (int i = 1; i < pathPoints.Count; i++)
        {
            Debug.DrawLine(pathPoints[i - 1], pathPoints[i], Color.green, 100f);
        }
    }
}
