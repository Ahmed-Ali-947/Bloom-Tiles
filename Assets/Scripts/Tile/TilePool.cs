using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bloom Tiles/Tile Pool")]
public class TilePool : ScriptableObject
{
    [SerializeField] private List<TileData> tiles = new();

    public IReadOnlyList<TileData> Tiles => tiles;
}