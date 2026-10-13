using System.Collections.Generic;
using UnityEngine;
using System;
public class LayoutGenerator
{
    private readonly FlowerGenerator flowerGenerator = new();
    public GeneratedLevel Generate(
        LevelParameters parameters,
        TilePool tilePool)
    {
        GeneratedLevel level = new();

        System.Random random = new System.Random(parameters.Seed);
        int totalTiles = parameters.GroupCount * 3;

        int minorTiles =
            parameters.MinorIslandCount * 9;

        int majorTiles =
            totalTiles - minorTiles;

        GenerateMajorIsland(
            level,
            parameters,
            random,
            majorTiles);

        GenerateMinorIslands(
            level,
            parameters,
            random,
            minorTiles);

        CenterLevel(level, parameters);

        flowerGenerator.AssignFlowers(
            level,
            tilePool,
            parameters.GroupCount,
            parameters.FlowerTypes,
            parameters.Seed);

        return level;
    }
    private void GenerateMajorIsland(
    GeneratedLevel level,
    LevelParameters parameters,
    System.Random random,
    int majorTiles)
    {
        IslandData island = new IslandData();

        level.AddIsland(island);

        int tilesRemaining = majorTiles;
        int layer = 0;

        while (tilesRemaining > 0)
        {
            island.AddLayerSettings(
                new LayerSettings(layer % 2 == 1, false));

            int capacity =
                parameters.MaxRowWidth *
                parameters.MaxRowsPerLayer;

            int tilesThisLayer =
                Mathf.Min(capacity, tilesRemaining);

            int placed = GenerateLayer(
                level,
                tilesThisLayer,
                layer,
                0,
                random,
                parameters);

            tilesRemaining -= placed;

            layer++;
        }
    }

    private void GenerateMinorIslands(
        GeneratedLevel level,
        LevelParameters parameters,
        System.Random random,
        int totalMinorTiles)
    {
        if (parameters.MinorIslandCount == 0)
            return;

        List<int> islandTileCounts =
            SplitTilesAcrossIslands(
                totalMinorTiles,
                parameters.MinorIslandCount);

        for (int i = 0; i < parameters.MinorIslandCount; i++)
        {
            GenerateMinorIsland(
                level,
                parameters,
                random,
                i,
                islandTileCounts[i]);
        }

    }

    private List<int> SplitTilesAcrossIslands(
    int totalTiles,
    int islandCount)
    {
        List<int> result = new();

        int remaining = totalTiles;

        for (int i = 0; i < islandCount; i++)
        {
            int islandsLeft = islandCount - i;

            int tiles =
                Mathf.RoundToInt((float)remaining / islandsLeft);

            // make sure every island has a multiple of 3
            tiles = (tiles / 3) * 3;

            // keep at least one group (3 tiles)
            tiles = Mathf.Max(3, tiles);

            result.Add(tiles);

            remaining -= tiles;
        }

        return result;
    }

    private void GenerateMinorIsland(
    GeneratedLevel level,
    LevelParameters parameters,
    System.Random random,
    int islandNumber,
    int tileCount)
    {
        IslandData island = new IslandData();

        // One layer per tile
        for (int layer = 0; layer < tileCount; layer++)
        {
            // No offsets at all
            island.AddLayerSettings(
                new LayerSettings(false, false));
        }

        float spacing = 3f;

        float startX =
            -(parameters.MinorIslandCount - 1)
            * spacing * 0.5f;

        float x =
            startX + islandNumber * spacing;

        float y = parameters.MinorIslandY;

        island.Move(new Vector2(x, y));

        level.AddIsland(island);

        int islandIndex = level.Islands.Count - 1;

        // One tile per layer, all at the same grid position
        for (int layer = 0; layer < tileCount; layer++)
        {
            level.AddTile(
                new TileSpawnData(
                    Vector2Int.zero,
                    layer,
                    islandIndex));
        }
    }

    private void CenterLevel(GeneratedLevel level, LevelParameters parameters)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (TileSpawnData tile in level.Tiles)
        {
            if (tile.IslandIndex != 0)
                continue;
            IslandData island = level.Islands[tile.IslandIndex];

            Vector2 position =
                island.BoardOffset +
                new Vector2(
                    tile.GridPosition.x * parameters.TileWidth,
                    tile.GridPosition.y * parameters.TileHeight);

            LayerSettings settings = island.LayerSettings[tile.Layer];

            if (settings.HorizontalOffset)
                position.x -= parameters.TileWidth * 0.5f;

            if (settings.VerticalOffset)
                position.y += parameters.TileHeight * 0.5f;

            minX = Mathf.Min(minX, position.x);
            maxX = Mathf.Max(maxX, position.x);

            minY = Mathf.Min(minY, position.y);
            maxY = Mathf.Max(maxY, position.y);
        }

        Vector2 center = new Vector2(
            (minX + maxX) * 0.5f,
            (minY + maxY) * 0.5f);

        Vector2 desiredCenter = parameters.BoardCenter;

        Vector2 offset = desiredCenter - center;

        foreach (IslandData island in level.Islands)
        {
            island.Move(offset);
        }
    }


    private int GenerateLayer(
        GeneratedLevel level,
        int tileCount,
        int layer,
        int islandIndex,
        System.Random random,
        LevelParameters parameters)
    {
        int remaining = tileCount;

        int y = 0;

        while (remaining > 0)
        {
            int maxWidth =
                Mathf.Min(parameters.MaxRowWidth, remaining);

            int rowWidth =
                random.Next(2, maxWidth + 1);

            if (remaining <= parameters.MaxRowWidth)
                rowWidth = remaining;

            int startX = -(rowWidth / 2);

            for (int x = 0; x < rowWidth; x++)
            {
                level.AddTile(
                    new TileSpawnData(
                        new Vector2Int(startX + x, y),
                        layer,
                        islandIndex));

                remaining--;
            }

            y++;

            if (y >= parameters.MaxRowsPerLayer)
                break;
        }

        return tileCount - remaining;
    }
}