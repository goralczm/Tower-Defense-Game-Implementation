using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class PathRenderer
{
    [Header("Async")]
    [SerializeField] private int _milisecondDelay = 100;
    [SerializeField] private int _minDelay = 5;
    [SerializeField] private int _speedUpFactor = 1;

    private PathSettings _pathSettings;
    private GenerationData _generationData;
    private TilemapSettings _tilemapSettings;
    private Tilemap _tilemap;
    private Vector2Int _entranceDir;
    private Vector2Int _exitDir;
    
    public static Action<TileBase> OnTileChanged;

    public void SetPathSettings(PathSettings pathSettings) => _pathSettings = pathSettings;
    public void SetGenerationData(GenerationData generationData) => _generationData = generationData;
    public void SetEntranceDir(Vector2Int entranceDir) => _entranceDir = entranceDir;
    public void SetExitDir(Vector2Int exitDir) => _exitDir = exitDir;
    public void SetTilemap(Tilemap tilemap) => _tilemap = tilemap;
    public void SetTilemapSettings(TilemapSettings tilemapSettings) => _tilemapSettings = tilemapSettings;
    public Vector3 Center => _tilemap.transform.position;
    
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
    public Bounds GetBounds()
    {
        return new Bounds(
            new Vector3(_generationData.MazeGenerationSettings.Width / 2f, _generationData.MazeGenerationSettings.Height / 2f, 0) + Center,
            new Vector3(_generationData.MazeGenerationSettings.Width + 1, _generationData.MazeGenerationSettings.Height + 1, 0));
    }

    public async Task RenderPath(MazeLayoutGenerator layout)
    {
        await ClearAllTiles();
        await Task.Delay(_milisecondDelay);

        await DrawPath(layout);
        await Task.Delay(_milisecondDelay);

        await DrawCorners(layout);
        await Task.Delay(_milisecondDelay);

        if (_pathSettings.EnforceRoundabouts)
            await DrawRoundabouts(layout, _generationData.Seed);
    }

    public void FillEntireMap()
    {
        for (int x = -1; x <= _generationData.MazeGenerationSettings.Width; x++)
        {
            for (int y = -1; y <= _generationData.MazeGenerationSettings.Height; y++)
            {
                SetTile(new(x, y), _tilemapSettings.Roundabout);
            }
        }
    }

    public async Task ClearAllTiles()
    {
        int speedUp = 0;
        BoundsInt bounds = _tilemap.cellBounds;

        for (int y = bounds.yMax; y >= bounds.yMin; y--)
        {
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                Vector3Int cellPos = new(x, y);
                if (y % 2 == 0)
                    cellPos = new(bounds.xMax - x - 1, y);

                if (_tilemap.GetTile(cellPos) != null)
                {
                    await Task.Delay(Mathf.Max(_minDelay, (_milisecondDelay / 2) - speedUp));
                    SetTile(cellPos, null);
                    speedUp += _speedUpFactor;
                }
            }
        }
    }

    private async Task DrawPath(MazeLayoutGenerator layoutGenerator)
    {
        int speedUp = 0;

        var curr = layoutGenerator.GetByCoords(_generationData.StartPoint);
        Vector2Int? prevPos = null;

        while (curr?.Next != null)
        {
            Vector2Int pos = curr.GetPosition();
            Vector3Int tilePos = new(pos.x, pos.y, 0);
            Vector2 dir = curr.Next.GetPosition() - pos;

            if (!prevPos.HasValue)
            {
                if (_generationData.IsTileOnEdge(pos))
                {
                    Vector3Int overflowTile = new(pos.x - _entranceDir.x, pos.y - _entranceDir.y, 0);
                    if (_entranceDir.x != 0)
                        SetTile(overflowTile, _tilemapSettings.Horizontal);
                    else if (_entranceDir.y != 0)
                        SetTile(overflowTile, _tilemapSettings.Vertical);

                    await Task.Delay(_milisecondDelay);
                }
            }

            if (dir.x != 0)
                SetTile(tilePos, _tilemapSettings.Horizontal);
            else if (dir.y != 0)
                SetTile(tilePos, _tilemapSettings.Vertical);

            await Task.Delay(Mathf.Max(_minDelay, _milisecondDelay - speedUp));

            speedUp += _speedUpFactor;

            prevPos = pos;
            curr = curr.Next;
        }

        if (prevPos.HasValue && curr != null)
        {
            Vector3Int lastTile = new(curr.X, curr.Y, 0);
            Vector2 lastDir = prevPos.Value - curr.GetPosition();

            if (lastDir.x != 0)
                SetTile(lastTile, _tilemapSettings.Horizontal);
            else if (lastDir.y != 0)
                SetTile(lastTile, _tilemapSettings.Vertical);

            await Task.Delay(_milisecondDelay);

            if (_generationData.IsTileOnEdge(curr.GetPosition()))
            {
                Vector3Int overflowTile = new(curr.X + _exitDir.x, curr.Y + _exitDir.y, 0);
                if (_exitDir.x != 0)
                    SetTile(overflowTile, _tilemapSettings.Horizontal);
                else if (_exitDir.y != 0)
                    SetTile(overflowTile, _tilemapSettings.Vertical);

                await Task.Delay(_milisecondDelay);
            }
        }
    }

    private async Task DrawCorners(MazeLayoutGenerator layoutGenerator)
    {
        int speedUp = 0;

        var curr = layoutGenerator.GetByCoords(_generationData.StartPoint);

        {
            Vector2Int pos = layoutGenerator.GetByCoords(_generationData.StartPoint).GetPosition();
            Vector2 dir = curr.Next.GetPosition() - pos;

            if (_entranceDir != dir && _generationData.IsTileOnEdge(pos))
            {
                Vector3Int cornerPos = new(_generationData.StartPoint.x, _generationData.StartPoint.y, 0);
                SetTile(cornerPos, GetCornerTile(_entranceDir, dir));

                await Task.Delay(_milisecondDelay);
            }
        }

        while (curr?.Next?.Next != null)
        {
            Vector2Int pos = curr.GetPosition();
            Vector2 dir = curr.Next.GetPosition() - pos;
            Vector2 nextDir = curr.Next.Next.GetPosition() - curr.Next.GetPosition();

            if (dir != nextDir)
            {
                Vector3Int cornerPos = new(curr.Next.X, curr.Next.Y, 0);
                SetTile(cornerPos, GetCornerTile(dir, nextDir));

                await Task.Delay(Mathf.Max(_minDelay, _milisecondDelay - speedUp));
                speedUp += _speedUpFactor;
            }

            curr = curr.Next;
        }

        if (curr != null)
        {
            Vector2Int pos = new(curr.X, curr.Y);
            Vector2 dir = _generationData.EndPoint - pos;
            Vector2 nextDir = (_generationData.EndPoint + _exitDir) - _generationData.EndPoint;

            if (nextDir != dir && _generationData.IsTileOnEdge(pos))
            {
                Vector3Int cornerPos = new(_generationData.EndPoint.x, _generationData.EndPoint.y, 0);
                SetTile(cornerPos, GetCornerTile(dir, nextDir));

                await Task.Delay(_milisecondDelay);
            }
        }
    }

    private async Task DrawRoundabouts(MazeLayoutGenerator layoutGenerator, int seed)
    {
        UnityEngine.Random.InitState(seed);
        var curr = layoutGenerator.GetByCoords(_generationData.StartPoint);

        int tilesAfterLastRoundabout = 200;

        while (curr?.Next?.Next != null)
        {
            Vector2Int pos = curr.GetPosition();
            Vector2 dir = curr.Next.GetPosition() - pos;
            Vector2 nextDir = curr.Next.Next.GetPosition() - curr.Next.GetPosition();

            if (dir != nextDir)
            {
                Vector3Int cornerPos = new(curr.Next.X, curr.Next.Y, 0);
                Tile corner = GetCornerTile(dir, nextDir);

                if (Randomizer.GetRandomBool(_pathSettings.RoundaboutProbability))
                {
                    if (_pathSettings.RandomizeRoundaboutSize)
                        tilesAfterLastRoundabout = await GenerateRandomRoundabout(cornerPos, corner, tilesAfterLastRoundabout);
                    else
                        tilesAfterLastRoundabout = await GenerateBiggestRoundabout(cornerPos, corner, tilesAfterLastRoundabout);
                }
            }

            curr = curr.Next;
            tilesAfterLastRoundabout++;
        }
    }

    private async Task<int> GenerateRandomRoundabout(Vector3Int cornerPos, Tile corner, int tilesAfterLastRoundabout)
    {
        List<int> randomSizes = new();

        for (int i = 2; i <= _pathSettings.BiggestRoundaboutSize; i++)
        {
            randomSizes.Add(i);
        }

        randomSizes.Shuffle();

        for (int i = 0; i < randomSizes.Count; i++)
        {
            if (await GenerateRoundaboutForCorner(cornerPos, corner, tilesAfterLastRoundabout, randomSizes[i]))
                return 0;
        }

        return tilesAfterLastRoundabout;
    }

    private async Task<int> GenerateBiggestRoundabout(Vector3Int cornerPos, Tile corner, int tilesAfterLastRoundabout)
    {
        for (int i = _pathSettings.BiggestRoundaboutSize; i >= 2; i--)
        {
            if (await GenerateRoundaboutForCorner(cornerPos, corner, tilesAfterLastRoundabout, i))
                return 0;
        }

        return tilesAfterLastRoundabout;
    }

    private async Task<bool> GenerateRoundaboutForCorner(Vector3Int cornerPos, Tile corner, int tilesAfterLastRoundabout, int size)
    {
        if (tilesAfterLastRoundabout < _pathSettings.MinimalTilesDistanceBetweenRoundabouts)
            return false;

        Tile[] corners = new[] { _tilemapSettings.RightTopCorner, _tilemapSettings.RightBottomCorner, _tilemapSettings.LeftBottomCorner, _tilemapSettings.LeftTopCorner };
        int orient = Array.IndexOf(corners, corner);
        if (orient < 0)
            throw new ArgumentException("Invalid corner tile", nameof(corner));

        List<Vector3Int> offsets = new List<Vector3Int>();
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                if (x != 0 || y != 0)
                    offsets.Add(new Vector3Int(x, -y, 0));

        foreach (var off in offsets)
        {
            var pos = cornerPos + RotateOffset(off, orient, size);
            if (!IsTileInBounds(pos))
                return false;

            if (!IsTileFree(pos))
                return false;
        }

        // diagonal = corner + offsetInt) * .5f

        Vector3Int offsetInt = RotateOffset(offsets[^1], orient, size);
        Vector2 offset = new(Mathf.Sign(offsetInt.x), Mathf.Sign(offsetInt.y));
        Vector2 centerOffset = offset * .5f * (size - 1);
        Vector2 center = new Vector2(cornerPos.x + centerOffset.x, cornerPos.y + centerOffset.y);
        Vector2 diagonalCorner = center + centerOffset;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                var local = new Vector3Int(x, -y, 0);
                var world = cornerPos + RotateOffset(local, orient, size);
                Tile toPlace = null;

                if (world.x == cornerPos.x || world.y == cornerPos.y || world.x == diagonalCorner.x || world.y == diagonalCorner.y)
                {
                    if (world.x == cornerPos.x && (world.y == diagonalCorner.y || world.y == cornerPos.y) ||
                        world.y == cornerPos.y && (world.x == diagonalCorner.x || world.x == cornerPos.x) ||
                        (world.x == diagonalCorner.x && world.y == diagonalCorner.y))
                    {
                        bool isLT = world.x > center.x && world.y < center.y;
                        bool isRT = world.x < center.x && world.y < center.y;
                        bool isRB = world.x < center.x && world.y > center.y;
                        bool isLB = world.x > center.x && world.y > center.y;

                        if (isLT) toPlace = corners[3];
                        if (isRT) toPlace = corners[0];
                        if (isRB) toPlace = corners[1];
                        if (isLB) toPlace = corners[2];
                    }
                    else
                    {
                        bool isHorizontal = world.y == cornerPos.y || world.y == diagonalCorner.y;
                        bool isVertical = world.x == cornerPos.x || world.x == diagonalCorner.x;

                        if (isHorizontal) toPlace = _tilemapSettings.Horizontal;
                        if (isVertical) toPlace = _tilemapSettings.Vertical;
                    }

                    SetTile(world, toPlace);
                    await Task.Delay(_milisecondDelay / 2);
                }
            }
        }

        SetTile(cornerPos, _tilemapSettings.Roundabout);

        return true;
    }

    private Vector3Int RotateOffset(Vector3Int local, int orient, int size)
    {
        switch (orient)
        {
            case 3: return local;
            case 2: return new Vector3Int(local.x, -local.y, 0);
            case 1: return new Vector3Int(-local.x, -local.y, 0);
            case 0: return new Vector3Int(-local.x, local.y, 0);
            default: return local;
        }
    }

    private void SetTile(Vector3Int tilePos, TileBase tileBase)
    {
        _tilemap.SetTile(tilePos, tileBase);

        OnTileChanged?.Invoke(tileBase);
    }

    private bool IsTileInBounds(Vector3Int tilePos)
    {
        return tilePos.x >= 0 && tilePos.x < _generationData.MazeGenerationSettings.Width &&
               tilePos.y >= 0 && tilePos.y < _generationData.MazeGenerationSettings.Height;
    }

    private bool IsTileFree(Vector3Int tilePos)
    {
        return _tilemap.GetTile(tilePos) == null;
    }

    private Tile GetCornerTile(Vector2 fromDir, Vector2 toDir)
    {
        if (fromDir == Vector2.left && toDir == Vector2.up) return _tilemapSettings.RightTopCorner;
        if (fromDir == Vector2.left && toDir == Vector2.down) return _tilemapSettings.RightBottomCorner;
        if (fromDir == Vector2.right && toDir == Vector2.up) return _tilemapSettings.LeftTopCorner;
        if (fromDir == Vector2.right && toDir == Vector2.down) return _tilemapSettings.LeftBottomCorner;
        if (fromDir == Vector2.up && toDir == Vector2.left) return _tilemapSettings.LeftBottomCorner;
        if (fromDir == Vector2.up && toDir == Vector2.right) return _tilemapSettings.RightBottomCorner;
        if (fromDir == Vector2.down && toDir == Vector2.left) return _tilemapSettings.LeftTopCorner;
        if (fromDir == Vector2.down && toDir == Vector2.right) return _tilemapSettings.RightTopCorner;

        return null;
    }
}
