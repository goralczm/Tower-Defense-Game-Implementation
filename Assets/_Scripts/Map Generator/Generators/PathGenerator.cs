using MapGenerator.Core;
using MapGenerator.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace MapGenerator.Generators
{
    [System.Serializable]
    public class PathGenerator : IGenerator
    {
        [Header("Debug")]
        [SerializeField] private bool _debug = true;

        private MapLayout _layout;
        private GenerationConfig _generationConfig;
        private Vector2Int _entranceDir;
        private Vector2Int _exitDir;
        private bool _renderOverflowTiles;

        public event Action<string> OnStatusChanged;

        public bool ShowDebug => _debug;

        public PathGenerator(MazeGenerationSettings mazeGenerationSettings, GenerationConfig generationData, bool renderOverflowTiles = false)
        {
            _generationConfig = generationData;
            _entranceDir = _generationConfig.GetAccessPointDir(mazeGenerationSettings.StartSide);
            _exitDir = -_generationConfig.GetAccessPointDir(mazeGenerationSettings.EndSide);
            _renderOverflowTiles = renderOverflowTiles;
        }

        public async Task<MapLayout> Generate(MapLayout layout, CancellationTokenSource cts)
        {
            _layout = layout;

            OnStatusChanged?.Invoke("Setting path tiles...");
            SetPathTiles();
            OnStatusChanged?.Invoke("Setting corner tiles...");
            SetCornerTiles();

            return _layout;
        }

        private void SetPathTiles()
        {
            var currNode = _layout.GetByCoords(_generationConfig.GridStartPoint);
            Vector2Int? prevNodePosition = null;

            while (currNode?.Next != null)
            {
                Vector2Int nodePosition = currNode.GetPosition();
                Vector2 dir = currNode.Next.GetPosition() - nodePosition;

                if (!prevNodePosition.HasValue && _renderOverflowTiles)
                {
                    if (_generationConfig.IsTileOnEdge(nodePosition))
                    {
                        Vector2Int overflowTile = new(nodePosition.x - _entranceDir.x, nodePosition.y - _entranceDir.y);
                        if (_entranceDir.x != 0)
                            _layout.SetTile(overflowTile, NodeType.Horizontal);
                        else if (_entranceDir.y != 0)
                            _layout.SetTile(overflowTile, NodeType.Vertical);

                        _layout.StartPoint = overflowTile;
                    }
                }

                if (dir.x != 0)
                    _layout.SetTile(nodePosition, NodeType.Horizontal);
                else if (dir.y != 0)
                    _layout.SetTile(nodePosition, NodeType.Vertical);

                prevNodePosition = nodePosition;
                currNode = currNode.Next;
            }

            if (prevNodePosition.HasValue && currNode != null)
            {
                Vector2Int lastTile = currNode.GetPosition();
                Vector2 lastDir = prevNodePosition.Value - lastTile;

                if (lastDir.x != 0)
                    _layout.SetTile(lastTile, NodeType.Horizontal);
                else if (lastDir.y != 0)
                    _layout.SetTile(lastTile, NodeType.Vertical);

                if (_renderOverflowTiles && _generationConfig.IsTileOnEdge(currNode.GetPosition()))
                {
                    Vector2Int overflowTile = new(currNode.X + _exitDir.x, currNode.Y + _exitDir.y);
                    if (_exitDir.x != 0)
                        _layout.SetTile(overflowTile, NodeType.Horizontal);
                    else if (_exitDir.y != 0)
                        _layout.SetTile(overflowTile, NodeType.Vertical);

                    _layout.EndPoint = overflowTile;
                }
            }
        }

        private void SetCornerTiles()
        {
            var curr = _layout.GetByCoords(_generationConfig.GridStartPoint);

            {
                Vector2Int pos = _layout.GetByCoords(_generationConfig.GridStartPoint).GetPosition();
                Vector2 dir = curr.Next.GetPosition() - pos;

                if (_entranceDir != dir && _generationConfig.IsTileOnEdge(pos))
                {
                    Vector2Int cornerPos = new(_generationConfig.GridStartPoint.x, _generationConfig.GridStartPoint.y);
                    _layout.SetTile(cornerPos, _layout.GetCornerTile(_entranceDir, dir));
                }
            }

            while (curr?.Next?.Next != null)
            {
                Vector2Int pos = curr.GetPosition();
                Vector2 dir = curr.Next.GetPosition() - pos;
                Vector2 nextDir = curr.Next.Next.GetPosition() - curr.Next.GetPosition();

                if (dir != nextDir)
                {
                    Vector2Int cornerPos = curr.Next.GetPosition();
                    _layout.SetTile(cornerPos, _layout.GetCornerTile(dir, nextDir));
                }

                curr = curr.Next;
            }

            if (curr != null)
            {
                Vector2Int pos = new(curr.X, curr.Y);
                Vector2 dir = _generationConfig.GridEndPoint - pos;
                Vector2 nextDir = (_generationConfig.GridEndPoint + _exitDir) - _generationConfig.GridEndPoint;

                if (nextDir != dir && _generationConfig.IsTileOnEdge(pos))
                {
                    Vector2Int cornerPos = new(_generationConfig.GridEndPoint.x, _generationConfig.GridEndPoint.y);
                    _layout.SetTile(cornerPos, _layout.GetCornerTile(dir, nextDir));
                }
            }
        }

        public void Cleanup()
        {
            //noop
        }

        public void DrawGizmos(DebugConfig debugConfig)
        {
            Gizmos.color = Color.magenta;
            var curr = debugConfig.Layout.GetByCoords(_generationConfig.GridStartPoint);
            
            if (curr.Next != null)
                GizmosHelpers.DrawGizmosArrow(debugConfig.GetPositionOnTilemap(curr.GetPosition()), debugConfig.GetPositionOnTilemap(curr.Next.GetPosition()), Color.magenta);

            while (curr.Next != null)
            {
                curr = curr.Next;
                if (curr.Next != null)
                    GizmosHelpers.DrawGizmosArrow(debugConfig.GetPositionOnTilemap(curr.GetPosition()), debugConfig.GetPositionOnTilemap(curr.Next.GetPosition()), Color.magenta);
            }
        }
    }
}
