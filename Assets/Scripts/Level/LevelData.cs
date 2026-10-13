using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bloom Tiles/Level")]
public class LevelData : ScriptableObject
{
    [SerializeField]
    private List<IslandData> islands = new();

    [SerializeField]
    private List<TileSpawnData> tiles = new();

    public IReadOnlyList<IslandData> Islands => islands;

    public IReadOnlyList<TileSpawnData> Tiles => tiles;
}