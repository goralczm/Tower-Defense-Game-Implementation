using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileAdjacency
{
    public TileBase Tile;
    public Vector2 Direction;
    public List<TileBase> PossibleAdjecentTiles;
}

[CreateAssetMenu(menuName = "Path Generation/Tilemap Settings", fileName = "New Tilemap Settings")]
public class TilemapSettings : ScriptableObject
{
    [Header("Tiles")]
    public Tile Roundabout;
    public Tile Horizontal;
    public Tile Vertical;
    public Tile LeftTopCorner;
    public Tile RightTopCorner;
    public Tile LeftBottomCorner;
    public Tile RightBottomCorner;
    
    [Header("Adjacency Settings")]
    public TileAdjacency[] AdjacencyRules;
}
