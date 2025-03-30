using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathsParent : MonoBehaviour
{
    [SerializeField] private int _seed;
    [SerializeField] private Vector2Int _startPos;
    [SerializeField] private Vector2Int _endPos;

    [SerializeField] private List<PathDisplay> _paths = new();

    int Width => _paths.Sum(p => p.GetWidth());
    int Height => _paths.Max(p => p.GetHeight());

    private void OnValidate()
    {
        for (int i = 0; i < _paths.Count; i++)
            _paths[i].SetSeed(_seed + i);

        _paths[0].SetStartPoint(_startPos);
        _paths[_paths.Count - 1].SetEndPoint(_endPos);
    }

    public void Randomize()
    {
        _seed = Random.Range(-100000, 100000);
        _startPos = new Vector2Int(0, Random.Range(0, Height));
        _endPos = new Vector2Int(Width, Random.Range(0, Height));

        OnValidate();
    }

    public List<Vector2> GetWaypoints()
    {
        List<Vector2> waypoints = new();

        foreach (var path in _paths)
            waypoints.AddRange(path.GetWaypoints());

        return waypoints;
    }
}
