using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class PathDisplay : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField] private PathSettings _pathSettings;
    [SerializeField] private GenerationData _generationData;

    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tile _roundabout;
    [SerializeField] private Tile _horizontal;
    [SerializeField] private Tile _vertical;
    [SerializeField] private Tile _ltCorner;
    [SerializeField] private Tile _rtCorner;
    [SerializeField] private Tile _lbCorner;
    [SerializeField] private Tile _rbCorner;
    [SerializeField] private Vector2Int _entranceDir;
    [SerializeField] private Vector2Int _exitDir;

    public void SetPathSettings(PathSettings pathSettings) => _pathSettings = pathSettings;
    public void SetEntranceDirection(Vector2Int dir) { _entranceDir = dir; }
    public void SetExitDirection(Vector2Int dir) { _exitDir = dir; }
    public Vector2 GetStartPointWorld()
    {
        if (_generationData.IsTileOnEdge(_generationData.StartPoint))
            return _tilemap.GetCellCenterWorld(new(_generationData.StartPoint.x - _entranceDir.x, _generationData.StartPoint.y - _entranceDir.y));

        return _tilemap.GetCellCenterWorld(new(_generationData.StartPoint.x, _generationData.StartPoint.y));
    }
    public Vector2 GetEndPointWorld()
    {
        if (_generationData.IsTileOnEdge(_generationData.EndPoint))
            return _tilemap.GetCellCenterWorld(new(_generationData.EndPoint.x - _exitDir.x, _generationData.EndPoint.y - _exitDir.y));

        return _tilemap.GetCellCenterWorld(new(_generationData.EndPoint.x, _generationData.EndPoint.y));
    }
    public void SetGenerationData(GenerationData generationData) => _generationData = generationData;

    public void GenerateTilemap(MazeGenerator generator)
    {
        _tilemap.ClearAllTiles();
        DrawPath(generator);
        DrawCorners(generator);
        if (_pathSettings.EnforceRoundabouts)
            DrawRoundabouts(generator, _generationData.Seed);
    }

    private void DrawPath(MazeGenerator generator)
    {
        var curr = generator.GetByCoords(_generationData.StartPoint);
        Vector2Int? prevPos = null;

        while (curr?.next != null)
        {
            Vector2Int pos = curr.GetPosition();
            Vector3Int tilePos = new(pos.x, pos.y, 0);
            Vector2 dir = curr.next.GetPosition() - pos;

            if (!prevPos.HasValue)
            {
                if (_generationData.IsTileOnEdge(pos))
                {
                    Vector3Int overflowTile = new(pos.x - _entranceDir.x, pos.y - _entranceDir.y, 0);
                    if (_entranceDir.x != 0)
                        _tilemap.SetTile(overflowTile, _horizontal);
                    else if (_entranceDir.y != 0)
                        _tilemap.SetTile(overflowTile, _vertical);
                }
            }

            if (dir.x != 0)
                _tilemap.SetTile(tilePos, _horizontal);
            else if (dir.y != 0)
                _tilemap.SetTile(tilePos, _vertical);

            prevPos = pos;
            curr = curr.next;
        }

        if (prevPos.HasValue && curr != null)
        {
            Vector3Int lastTile = new(curr.x, curr.y, 0);
            Vector2 lastDir = prevPos.Value - curr.GetPosition();

            if (lastDir.x != 0)
                _tilemap.SetTile(lastTile, _horizontal);
            else if (lastDir.y != 0)
                _tilemap.SetTile(lastTile, _vertical);

            if (_generationData.IsTileOnEdge(curr.GetPosition()))
            {
                Vector3Int overflowTile = new(curr.x + _exitDir.x, curr.y + _exitDir.y, 0);
                if (_exitDir.x != 0)
                    _tilemap.SetTile(overflowTile, _horizontal);
                else if (_exitDir.y != 0)
                    _tilemap.SetTile(overflowTile, _vertical);
            }
        }
    }

    private void DrawCorners(MazeGenerator generator)
    {
        var curr = generator.GetByCoords(_generationData.StartPoint);

        {
            Vector2Int pos = generator.GetByCoords(_generationData.StartPoint).GetPosition();
            Vector2 dir = curr.next.GetPosition() - pos;

            if (_entranceDir != dir && _generationData.IsTileOnEdge(pos))
            {
                Vector3Int cornerPos = new(_generationData.StartPoint.x, _generationData.StartPoint.y, 0);
                _tilemap.SetTile(cornerPos, GetCornerTile(_entranceDir, dir));
            }
        }

        while (curr?.next?.next != null)
        {
            Vector2Int pos = curr.GetPosition();
            Vector2 dir = curr.next.GetPosition() - pos;
            Vector2 nextDir = curr.next.next.GetPosition() - curr.next.GetPosition();

            if (dir != nextDir)
            {
                Vector3Int cornerPos = new(curr.next.x, curr.next.y, 0);
                _tilemap.SetTile(cornerPos, GetCornerTile(dir, nextDir));
            }

            curr = curr.next;
        }

        if (curr != null)
        {
            Vector2Int pos = new(curr.x, curr.y);
            Vector2 dir = _generationData.EndPoint - pos;
            Vector2 nextDir = (_generationData.EndPoint + _exitDir) - _generationData.EndPoint;

            if (nextDir != dir && _generationData.IsTileOnEdge(pos))
            {
                Vector3Int cornerPos = new(_generationData.EndPoint.x, _generationData.EndPoint.y, 0);
                _tilemap.SetTile(cornerPos, GetCornerTile(dir, nextDir));
            }
        }
    }

    private void DrawRoundabouts(MazeGenerator generator, int seed)
    {
        UnityEngine.Random.InitState(seed);
        var curr = generator.GetByCoords(_generationData.StartPoint);

        int tilesAfterLastRoundabout = 200;

        while (curr?.next?.next != null)
        {
            Vector2Int pos = curr.GetPosition();
            Vector2 dir = curr.next.GetPosition() - pos;
            Vector2 nextDir = curr.next.next.GetPosition() - curr.next.GetPosition();

            if (dir != nextDir)
            {
                Vector3Int cornerPos = new(curr.next.x, curr.next.y, 0);
                Tile corner = GetCornerTile(dir, nextDir);

                if (GenerateRoundaboutForCorner(cornerPos, corner, tilesAfterLastRoundabout))
                    tilesAfterLastRoundabout = 0;
            }

            curr = curr.next;
            tilesAfterLastRoundabout++;
        }
    }

    private bool GenerateRoundaboutForCorner(Vector3Int cornerPos, Tile corner, int tilesAfterLastRoundabout)
    {
        Vector3Int[] roundaboutOffsets = null;

        Tile[] roundabout = new[] { _rtCorner, _rbCorner, _lbCorner, _ltCorner };

        if (corner == _ltCorner)
            roundaboutOffsets = new Vector3Int[] { Vector3Int.down, Vector3Int.zero, Vector3Int.right, Vector3Int.right + Vector3Int.down };
        else if (corner == _lbCorner)
            roundaboutOffsets = new Vector3Int[] { Vector3Int.zero, Vector3Int.up, Vector3Int.right + Vector3Int.up, Vector3Int.right };
        else if (corner == _rtCorner)
            roundaboutOffsets = new Vector3Int[] { Vector3Int.left + Vector3Int.down, Vector3Int.left, Vector3Int.zero, Vector3Int.down };
        else if (corner == _rbCorner)
            roundaboutOffsets = new Vector3Int[] { Vector3Int.left, Vector3Int.left + Vector3Int.up, Vector3Int.up, Vector3Int.zero };
        
        for (int i = 0; i < roundaboutOffsets.Length; i++)
        {
            if (roundaboutOffsets[i] == Vector3Int.zero) continue;
            
            if (!IsTileInBounds(cornerPos + roundaboutOffsets[i]))
                return false;

            if (!IsTileFree(cornerPos + roundaboutOffsets[i]))
                return false;
        }

        if (tilesAfterLastRoundabout < _pathSettings.MinimalTilesDistanceBetweenRoundabouts) return false;

        if (UnityEngine.Random.Range(0, 101) > _pathSettings.RoundaboutPercentageChance) return false;

        for (int i = 0; i < roundabout.Length; i++)
        {
            Vector3Int offsetedCorner = cornerPos + roundaboutOffsets[i];
            _tilemap.SetTile(offsetedCorner, roundabout[i]);
        }

        _tilemap.SetTile(cornerPos, _roundabout);
        return true;
    }

    private bool IsTileInBounds(Vector3Int tilePos)
    {
        return tilePos.x >= 0 && tilePos.x < _generationData.GenerationDataBase.Width &&
               tilePos.y >= 0 && tilePos.y < _generationData.GenerationDataBase.Height;
    }

    private bool IsTileFree(Vector3Int tilePos)
    {
        return _tilemap.GetTile(tilePos) == null;
    }

    private Tile GetCornerTile(Vector2 fromDir, Vector2 toDir)
    {
        if (fromDir == Vector2.left && toDir == Vector2.up) return _rtCorner;
        if (fromDir == Vector2.left && toDir == Vector2.down) return _rbCorner;
        if (fromDir == Vector2.right && toDir == Vector2.up) return _ltCorner;
        if (fromDir == Vector2.right && toDir == Vector2.down) return _lbCorner;
        if (fromDir == Vector2.up && toDir == Vector2.left) return _lbCorner;
        if (fromDir == Vector2.up && toDir == Vector2.right) return _rbCorner;
        if (fromDir == Vector2.down && toDir == Vector2.left) return _ltCorner;
        if (fromDir == Vector2.down && toDir == Vector2.right) return _rtCorner;

        return null;
    }
}
