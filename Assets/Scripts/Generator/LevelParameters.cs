using UnityEngine;

[System.Serializable]
public class LevelParameters
{
    public int Seed;

    public int GroupCount;

    public int FlowerTypes;

    public int MajorIslandLayers;

    public int MinorIslandCount;

    public int MinorIslandLayers;
    public float MinorIslandY;

    public float TileWidth;
    public float TileHeight;
    public int MaxRowWidth;
    public int MaxRowsPerLayer;
    public Vector2 BoardCenter;
}