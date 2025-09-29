using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public class RoundaboutsGenerator : IGenerator
    {
        private MapLayout _layout;
        private PathSettings _pathSettings;
        private GenerationConfig _generationData;

        public RoundaboutsGenerator(PathSettings pathSettings, GenerationConfig generationData)
        {
            _pathSettings = pathSettings;
            _generationData = generationData;
        }

        public MapLayout Generate(MapLayout layout)
        {
            _layout = layout;

            if (_pathSettings.EnforceRoundabouts)
                DrawRoundabouts(_generationData.Seed);

            return _layout;
        }

        private void DrawRoundabouts(int seed)
        {
            UnityEngine.Random.InitState(seed);
            var curr = _layout.GetByCoords(_generationData.GridStartPoint);

            int tilesAfterLastRoundabout = 200;

            while (curr?.Next?.Next != null)
            {
                Vector2Int pos = curr.GetPosition();
                Vector2 dir = curr.Next.GetPosition() - pos;
                Vector2 nextDir = curr.Next.Next.GetPosition() - curr.Next.GetPosition();

                if (dir != nextDir)
                {
                    Vector2Int cornerPos = curr.Next.GetPosition();
                    NodeType corner = _layout.GetCornerTile(dir, nextDir);

                    if (Randomizer.GetRandomBool(_pathSettings.RoundaboutProbability))
                    {
                        if (_pathSettings.RandomizeRoundaboutSize)
                            tilesAfterLastRoundabout = GenerateRandomRoundabout(cornerPos, corner, tilesAfterLastRoundabout);
                        else
                            tilesAfterLastRoundabout = GenerateBiggestRoundabout(cornerPos, corner, tilesAfterLastRoundabout);
                    }
                }

                curr = curr.Next;
                tilesAfterLastRoundabout++;
            }
        }

        private int GenerateRandomRoundabout(Vector2Int cornerPos, NodeType corner, int tilesAfterLastRoundabout)
        {
            List<int> randomSizes = new();

            for (int i = 2; i <= _pathSettings.BiggestRoundaboutSize; i++)
            {
                randomSizes.Add(i);
            }

            randomSizes.Shuffle();

            for (int i = 0; i < randomSizes.Count; i++)
            {
                if (GenerateRoundaboutForCorner(cornerPos, corner, tilesAfterLastRoundabout, randomSizes[i]))
                    return 0;
            }

            return tilesAfterLastRoundabout;
        }

        private int GenerateBiggestRoundabout(Vector2Int cornerPos, NodeType corner, int tilesAfterLastRoundabout)
        {
            for (int i = _pathSettings.BiggestRoundaboutSize; i >= 2; i--)
            {
                if (GenerateRoundaboutForCorner(cornerPos, corner, tilesAfterLastRoundabout, i))
                    return 0;
            }

            return tilesAfterLastRoundabout;
        }

        private bool GenerateRoundaboutForCorner(Vector2Int cornerPos, NodeType corner, int tilesAfterLastRoundabout, int size)
        {
            if (tilesAfterLastRoundabout < _pathSettings.MinimalTilesDistanceBetweenRoundabouts)
                return false;

            NodeType[] corners = new[] { NodeType.RightTopCorner, NodeType.RightBottomCorner, NodeType.LeftBottomCorner, NodeType.LeftTopCorner };
            int orient = Array.IndexOf(corners, corner);
            if (orient < 0)
                throw new ArgumentException("Invalid corner tile", nameof(corner));

            List<Vector2Int> offsets = new List<Vector2Int>();
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    if (x != 0 || y != 0)
                        offsets.Add(new(x, -y));

            foreach (var off in offsets)
            {
                var pos = cornerPos + RotateOffset(off, orient, size);
                if (!IsTileInBounds(pos))
                    return false;

                if (!IsTileFree(pos))
                    return false;
            }

            // diagonal = corner + offsetInt) * .5f

            Vector2Int offsetInt = RotateOffset(offsets[^1], orient, size);
            Vector2 offset = new(Mathf.Sign(offsetInt.x), Mathf.Sign(offsetInt.y));
            Vector2 centerOffset = offset * (.5f * (size - 1));
            Vector2 center = new Vector2(cornerPos.x + centerOffset.x, cornerPos.y + centerOffset.y);
            Vector2 diagonalCorner = center + centerOffset;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vector2Int local = new(x, -y);
                    var world = cornerPos + RotateOffset(local, orient, size);
                    NodeType toPlace = NodeType.Empty;

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

                            if (isHorizontal) toPlace = NodeType.Horizontal;
                            if (isVertical) toPlace = NodeType.Vertical;
                        }

                        _layout.SetTile(world, toPlace);
                    }
                }
            }

            _layout.SetTile(cornerPos, NodeType.Roundabout);

            return true;
        }

        private Vector2Int RotateOffset(Vector2Int local, int orient, int size)
        {
            switch (orient)
            {
                case 3: return local;
                case 2: return new(local.x, -local.y);
                case 1: return new(-local.x, -local.y);
                case 0: return new(-local.x, local.y);
                default: return local;
            }
        }

        private bool IsTileInBounds(Vector2Int tilePos)
        {
            return tilePos.x >= 0 && tilePos.x < _generationData.MazeGenerationSettings.Width &&
                   tilePos.y >= 0 && tilePos.y < _generationData.MazeGenerationSettings.Height;
        }

        private bool IsTileFree(Vector2Int tilePos)
        {
            return _layout.GetByCoords(tilePos).Type == NodeType.Empty;
        }

        public void Cleanup()
        {
            //noop
        }
    }
}
