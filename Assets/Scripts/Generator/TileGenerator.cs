using System;
using System.Collections.Generic;

public class FlowerGenerator
{
    public void AssignFlowers(
        GeneratedLevel level,
        TilePool tilePool,
        int groupCount,
        int flowerTypes,
        int seed)
    {
        Random random = new(seed);

        List<TileData> flowerList =
            BuildFlowerList(
                groupCount,
                tilePool,
                flowerTypes,
                random);

        Shuffle(flowerList, random);

        for (int i = 0; i < level.Tiles.Count; i++)
        {
            level.Tiles[i].SetTileData(flowerList[i]);
        }
    }

    private List<TileData> BuildFlowerList(
        int groupCount,
        TilePool pool,
        int flowerTypes,
        Random random)
    {
        List<TileData> flowers = new();

        int groups = groupCount;
        List<int> groupCounts = GenerateGroupCounts(
            groups,
            flowerTypes,
            random);

        for (int flower = 0; flower < flowerTypes; flower++)
        {
            int tileCount = groupCounts[flower] * 3;

            for (int i = 0; i < tileCount; i++)
            {
                flowers.Add(pool.Tiles[flower]);
            }
        }

        return flowers;
    }

    private List<int> GenerateGroupCounts(
        int groups,
        int flowerTypes,
        Random random)
    {
        List<int> counts = new();

        for (int i = 0; i < flowerTypes; i++)
            counts.Add(1);

        groups -= flowerTypes;

        while (groups > 0)
        {
            int index = random.Next(flowerTypes);

            counts[index]++;

            groups--;
        }

        return counts;
    }

    private void Shuffle<T>(
        List<T> list,
        Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);

            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}