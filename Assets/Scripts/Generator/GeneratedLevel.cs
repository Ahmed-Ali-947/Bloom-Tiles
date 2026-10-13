using System.Collections.Generic;

public class GeneratedLevel
{
    private readonly List<IslandData> islands = new();
    private readonly List<TileSpawnData> tiles = new();

    public IReadOnlyList<IslandData> Islands => islands;
    public IReadOnlyList<TileSpawnData> Tiles => tiles;

    public void AddIsland(IslandData island)
    {
        islands.Add(island);
    }

    public void AddTile(TileSpawnData tile)
    {
        tiles.Add(tile);
    }
}