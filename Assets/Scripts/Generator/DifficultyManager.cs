using UnityEngine;

public class DifficultyManager
{
    public LevelParameters GetParameters(int level)
    {
        LevelParameters parameters = new();

        parameters.Seed = level;

        parameters.GroupCount =
            Mathf.Clamp(8 + level * 2, 12, 80);

        parameters.FlowerTypes =
            Mathf.Clamp(4 + level / 8, 4, 12);

        parameters.FlowerTypes =
            Mathf.Min(parameters.FlowerTypes,
                      parameters.GroupCount);

        parameters.MaxRowWidth = 5;

        parameters.MaxRowsPerLayer =
            Mathf.Clamp(4 + level / 20, 4, 6);

        parameters.MinorIslandCount =
            Mathf.Clamp(level / 12, 1, 4);

        parameters.MinorIslandLayers =
            Mathf.Clamp(2 + level / 15, 2, 4);

        return parameters;
    }
}