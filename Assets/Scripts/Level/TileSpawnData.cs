using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileSpawnData
{
    [SerializeField] private Vector2Int gridPosition;
    [SerializeField] private int layer;
    [SerializeField] private int islandIndex;

    private TileData tileData;

    public Vector2Int GridPosition => gridPosition;
    public int Layer => layer;
    public int IslandIndex => islandIndex;
    public TileData TileData => tileData;

    public void SetTileData(TileData data)
    {
        tileData = data;
    }

    public TileSpawnData(Vector2Int position, int layer, int islandIndex)
    {
        gridPosition = position;
        this.layer = layer;
        this.islandIndex = islandIndex;
    }
}