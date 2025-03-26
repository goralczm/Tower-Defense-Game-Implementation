using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PathGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private Vector2Int _startPosition;

    [Header("Instances")]
    [SerializeField] private Grid _grid;
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private LineRenderer _line;

    [Header("Debug")]
    [SerializeField] private float _pointsSize = .25f;

    private Dictionary<(float, float), Color> _sampledColors = new();
    private List<Vector2> _waypoints = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(SampleTexture());
    }

    IEnumerator SampleTexture()
    {
        yield return new WaitForEndOfFrame();

        _sampledColors.Clear();
        _waypoints.Clear();

        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        for (int y = _startPosition.y + _height - 1; y >= _startPosition.y; y--)
        {
            Color color = Color.white;
            Vector2 cell = Vector2.zero;

            if (y % 2 == 0)
            {
                for (int x = _startPosition.x; x < _startPosition.x + _width; x++)
                {
                    cell = new Vector2(x, y);
                    color = SamplePixelAtCell(screenTexture, x, y);

                    if (color.r <= .1f)
                    {
                        Vector3 cellGridPos = _grid.GetCellCenterWorld(_grid.WorldToCell(cell));

                        _waypoints.Add(cellGridPos);
                    }
                }
            }
            else
            {
                for (int x = _startPosition.x + _width - 1; x >= _startPosition.x; x--)
                {
                    cell = new Vector2(x, y);
                    color = SamplePixelAtCell(screenTexture, x, y);

                    if (color.r <= .1f)
                    {
                        Vector3 cellGridPos = _grid.GetCellCenterWorld(_grid.WorldToCell(cell));

                        _waypoints.Add(cellGridPos);
                    }
                }
            }


        }

        if (_splineContainer.Splines.Count > 0)
            _splineContainer.RemoveSpline(_splineContainer.Splines[0]);

        _line.positionCount = _waypoints.Count;

        Spline spline = _splineContainer.AddSpline();
        for (int i = 0; i < _waypoints.Count; i++)
        {
            spline.Add(new BezierKnot(new Unity.Mathematics.float3(_waypoints[i].x, _waypoints[i].y, 0)), TangentMode.AutoSmooth);
            _line.SetPosition(i, _waypoints[i]);
        }

        Destroy(screenTexture);
    }

    private Color SamplePixelAtCell(Texture2D screenTexture, int x, int y)
    {
        Vector2 cell = new Vector3(x, y);
        Vector3 cellGridPos = _grid.GetCellCenterWorld(_grid.WorldToCell(cell));

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(cellGridPos);
        Color color = GetScreenColor(screenTexture, screenPosition);
        _sampledColors.Add((x, y), color);

        return color;
    }

    private Color GetScreenColor(Texture2D screenTexture, Vector2 screenPosition)
    {
        screenTexture.ReadPixels(new Rect(screenPosition.x, screenPosition.y, 1, 1), 0, 0);
        screenTexture.Apply();

        Color color = screenTexture.GetPixel(0, 0);

        return color;
    }

    private void OnDrawGizmos()
    {
        if (_sampledColors.Count == 0)
        {
            Gizmos.color = Color.green;
            for (int x = _startPosition.x; x < _startPosition.x + _width; x++)
            {
                for (int y = _startPosition.y; y < _startPosition.y + _height; y++)
                {
                    Vector2 cell = new Vector3(x, y);
                    Vector3 cellGridPos = _grid.GetCellCenterWorld(_grid.WorldToCell(cell));

                    Gizmos.DrawWireSphere(cellGridPos, _pointsSize);
                }
            }
            return;
        }

        for (int x = _startPosition.x; x < _startPosition.x + _width; x++)
        {
            for (int y = _startPosition.y; y < _startPosition.y + _height; y++)
            {
                Vector2 cell = new Vector3(x, y);
                Vector3 cellGridPos = _grid.GetCellCenterWorld(_grid.WorldToCell(cell));

                if (_sampledColors.TryGetValue((x, y), out var color))
                {
                    if (_waypoints.Contains(cellGridPos))
                        Gizmos.color = Color.red;
                    else
                        Gizmos.color = _sampledColors[(x, y)];
                    Gizmos.DrawWireSphere(cellGridPos, _pointsSize);
                }
            }
        }
    }
}
