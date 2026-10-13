using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IslandData
{
    [SerializeField] private Vector2 boardOffset;

    [SerializeField] private List<LayerSettings> layerSettings = new();

    public Vector2 BoardOffset => boardOffset;

    public IReadOnlyList<LayerSettings> LayerSettings => layerSettings;

    public void AddLayerSettings(LayerSettings settings)
    {
        layerSettings.Add(settings);
    }

    public void Move(Vector2 offset)
    {
        boardOffset += offset;
    }
}