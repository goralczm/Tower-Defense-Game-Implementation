using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class PathRenderer
{
    private PathSettings _pathSettings;
    private GenerationConfig _generationData;
    private TilemapSettings _tilemapSettings;
    private Tilemap _pathTilemap;
    private Vector2Int _entranceDir;
    private Vector2Int _exitDir;
    private bool _renderOverflowTiles;
    
    public static Func<TileBase, Task> OnTileChangedAsync;
    
    public PathRenderer(Tilemap pathTilemap, PathSettings pathSettings, TilemapSettings tilemapSettings, GenerationConfig generationData, Vector2Int entranceDir, Vector2Int exitDir)
    {
        _pathTilemap = pathTilemap;
        _pathSettings = pathSettings;
        _tilemapSettings = tilemapSettings;
        _generationData = generationData;
        _entranceDir = entranceDir;
        _exitDir = exitDir;
    }

    public Vector2 GetStartPointWorld()
    {
        if (_renderOverflowTiles && _generationData.IsTileOnEdge(_generationData.GridStartPoint))
            return _pathTilemap.GetCellCenterWorld(new(_generationData.GridStartPoint.x - _entranceDir.x, _generationData.GridStartPoint.y - _entranceDir.y));

        return _pathTilemap.GetCellCenterWorld(new(_generationData.GridStartPoint.x, _generationData.GridStartPoint.y));
    }
    
    public Vector2 GetEndPointWorld()
    {
        if (_renderOverflowTiles && _generationData.IsTileOnEdge(_generationData.GridEndPoint))
            return _pathTilemap.GetCellCenterWorld(new(_generationData.GridEndPoint.x - _exitDir.x, _generationData.GridEndPoint.y - _exitDir.y));

        return _pathTilemap.GetCellCenterWorld(new(_generationData.GridEndPoint.x, _generationData.GridEndPoint.y));
    }

    public async Task RenderPath(MazeLayoutGenerator layout, bool renderOverflowTiles = false)
    {
        await ClearAllTiles();
        await DrawPath(layout, renderOverflowTiles);
        await DrawCorners(layout);

        if (_pathSettings.EnforceRoundabouts)
            await DrawRoundabouts(layout, _generationData.Seed);
    }

    public async Task FillEntireMap()
    {
        for (int x = -1; x <= _generationData.MazeGenerationSettings.Width; x++)
        {
            for (int y = -1; y <= _generationData.MazeGenerationSettings.Height; y++)
            {
                await SetTile(new(x, y), _tilemapSettings.Roundabout);
            }
        }
    }

    public async Task ClearAllTiles()
    {
        BoundsInt bounds = _pathTilemap.cellBounds;

        for (int y = bounds.yMax; y >= bounds.yMin; y--)
        {
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                Vector3Int cellPos = new(x, y);
                if (y % 2 == 0)
                    cellPos = new(bounds.xMax - x - 1, y);

                if (_pathTilemap.GetTile(cellPos) != null)
                {
                    await SetTile(cellPos, null);
                }
            }
        }
    }

    private async Task DrawPath(MazeLayoutGenerator layout, bool renderOverflowTiles = false)
    {
        _renderOverflowTiles = renderOverflowTiles;
        
        var currNode = layout.GetByCoords(_generationData.GridStartPoint);
        Vector2Int? prevNodePosition = null;

        while (currNode?.Next != null)
        {
            Vector2Int nodePosition = currNode.GetPosition();
            Vector3Int tilePosition = new(nodePosition.x, nodePosition.y, 0);
            Vector2 dir = currNode.Next.GetPosition() - nodePosition;

            if (!prevNodePosition.HasValue && renderOverflowTiles)
            {
                if (_generationData.IsTileOnEdge(nodePosition))
                {
                    Vector3Int overflowTile = new(nodePosition.x - _entranceDir.x, nodePosition.y - _entranceDir.y, 0);
                    if (_entranceDir.x != 0)
                        await SetTile(overflowTile, _tilemapSettings.Horizontal);
                    else if (_entranceDir.y != 0)
                        await SetTile(overflowTile, _tilemapSettings.Vertical);
                }
            }

            if (dir.x != 0)
                await SetTile(tilePosition, _tilemapSettings.Horizontal);
            else if (dir.y != 0)
                await SetTile(tilePosition, _tilemapSettings.Vertical);

            prevNodePosition = nodePosition;
            currNode = currNode.Next;
        }

        if (prevNodePosition.HasValue && currNode != null)
        {
            Vector3Int lastTile = new(currNode.X, currNode.Y, 0);
            Vector2 lastDir = prevNodePosition.Value - currNode.GetPosition();

            if (lastDir.x != 0)
                await SetTile(lastTile, _tilemapSettings.Horizontal);
            else if (lastDir.y != 0)
                await SetTile(lastTile, _tilemapSettings.Vertical);

            if (renderOverflowTiles && _generationData.IsTileOnEdge(currNode.GetPosition()))
            {
                Vector3Int overflowTile = new(currNode.X + _exitDir.x, currNode.Y + _exitDir.y, 0);
                if (_exitDir.x != 0)
                    await SetTile(overflowTile, _tilemapSettings.Horizontal);
                else if (_exitDir.y != 0)
                    await SetTile(overflowTile, _tilemapSettings.Vertical);
            }
        }
    }

    private async Task DrawCorners(MazeLayoutGenerator layout)
    {
        var curr = layout.GetByCoords(_generationData.GridStartPoint);

        {
            Vector2Int pos = layout.GetByCoords(_generationData.GridStartPoint).GetPosition();
            Vector2 dir = curr.Next.GetPosition() - pos;

            if (_entranceDir != dir && _generationData.IsTileOnEdge(pos))
            {
                Vector3Int cornerPos = new(_generationData.GridStartPoint.x, _generationData.GridStartPoint.y, 0);
                await SetTile(cornerPos, GetCornerTile(_entranceDir, dir));
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
                await SetTile(cornerPos, GetCornerTile(dir, nextDir));
            }

            curr = curr.Next;
        }

        if (curr != null)
        {
            Vector2Int pos = new(curr.X, curr.Y);
            Vector2 dir = _generationData.GridEndPoint - pos;
            Vector2 nextDir = (_generationData.GridEndPoint + _exitDir) - _generationData.GridEndPoint;

            if (nextDir != dir && _generationData.IsTileOnEdge(pos))
            {
                Vector3Int cornerPos = new(_generationData.GridEndPoint.x, _generationData.GridEndPoint.y, 0);
                await SetTile(cornerPos, GetCornerTile(dir, nextDir));
            }
        }
    }

    private async Task DrawRoundabouts(MazeLayoutGenerator layout, int seed)
    {
        UnityEngine.Random.InitState(seed);
        var curr = layout.GetByCoords(_generationData.GridStartPoint);

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
        Vector2 centerOffset = offset * (.5f * (size - 1));
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

                    await SetTile(world, toPlace);
                    //await Task.Delay(_milisecondDelay / 2);
                }
            }
        }

        await SetTile(cornerPos, _tilemapSettings.Roundabout);

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

    private async Task SetTile(Vector3Int tilePos, TileBase tileBase)
    {
        _pathTilemap.SetTile(tilePos, tileBase);

        if (OnTileChangedAsync != null)
            await OnTileChangedAsync.Invoke(tileBase);
    }

    private bool IsTileInBounds(Vector3Int tilePos)
    {
        return tilePos.x >= 0 && tilePos.x < _generationData.MazeGenerationSettings.Width &&
               tilePos.y >= 0 && tilePos.y < _generationData.MazeGenerationSettings.Height;
    }

    private bool IsTileFree(Vector3Int tilePos)
    {
        return _pathTilemap.GetTile(tilePos) == null;
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
