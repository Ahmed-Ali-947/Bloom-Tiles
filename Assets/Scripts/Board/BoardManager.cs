using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    public event Action LevelCompleted;

    [Header("References")]
    [SerializeField] private TilePool tilePool;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform boardRoot;
    [SerializeField] private TrayManager trayManager;

    [Header("Grid")]
    [SerializeField] private float tileWidth = 1f;
    [SerializeField] private float tileHeight = 1f;

    private readonly List<Tile> spawnedTiles = new();
    private int remainingTiles;
    private GeneratedLevel level;
    private void Start()
    {
        LayoutGenerator generator = new LayoutGenerator();

        DifficultyManager difficultyManager = new DifficultyManager();

        DifficultyManager difficulty = new();

        LevelParameters parameters =
            difficulty.GetParameters(
                LevelManager.Instance.CurrentLevel);

        parameters.TileWidth = tileWidth;
        parameters.TileHeight = tileHeight;
        parameters.BoardCenter = new Vector2(0f, 0f);
        parameters.MinorIslandY = -2f;

        level = generator.Generate(parameters, tilePool);

        SpawnTiles();

        BuildRelationships();
        boardRoot.localScale = new Vector3(0.75f, 0.75f, 1f);
    }
    private void OnEnable()
    {
        trayManager.TileAddedToTray += NotifyTileRemoved;
    }

    private void OnDisable()
    {
        trayManager.TileAddedToTray -= NotifyTileRemoved;
    }
    private void SpawnTiles()
    {
        foreach (TileSpawnData tileData in level.Tiles)
        {
            IslandData island = level.Islands[tileData.IslandIndex];

            Vector2 position =
                island.BoardOffset +
                new Vector2(
                    tileData.GridPosition.x * tileWidth,
                    tileData.GridPosition.y * tileHeight);

            LayerSettings settings = island.LayerSettings[tileData.Layer];

            Vector2 offset = Vector2.zero;

            if (settings.HorizontalOffset)
                offset.x = -tileWidth * 0.5f;

            if (settings.VerticalOffset)
                offset.y = tileHeight * 0.5f;

            position += offset;

            Tile tile = Instantiate(tilePrefab, position, Quaternion.identity, boardRoot);

            tile.Initialize(tileData.TileData, tileData.Layer);

            tile.SetSortingOrder(CalculateSortingOrder(tileData));

            spawnedTiles.Add(tile);
        }

        remainingTiles = spawnedTiles.Count;
    }

    private void BuildRelationships()
    {
        spawnedTiles.RemoveAll(t => t == null);
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Tile"));
        filter.useLayerMask = true;

        List<Collider2D> results = new();

        foreach (Tile lowerTile in spawnedTiles)
        {
            if (lowerTile == null)
                continue;

            if (!lowerTile.gameObject.activeSelf)
                continue;

            results.Clear();

            lowerTile.Collider.Overlap(filter, results);

            foreach (Collider2D collider in results)
            {

                Tile upperTile = collider.GetComponent<Tile>();

                if (upperTile == null)
                    continue;

                if (!upperTile.gameObject.activeSelf)
                    continue;

                if (upperTile == lowerTile)
                    continue;

                if (upperTile.Layer <= lowerTile.Layer)
                    continue;

                lowerTile.AddBlocker(upperTile);
                upperTile.AddCoveredTile(lowerTile);
            }
        }

        foreach (Tile tile in spawnedTiles)
            tile.RefreshState();
    }

    private int CalculateSortingOrder(TileSpawnData data)
    {
        return data.Layer * 1000
               - data.GridPosition.y * 100
               + data.GridPosition.x;
    }

    public void HandleTileSelected(Tile tile)
    {
        trayManager.AddTile(tile);
    }

    public void NotifyTileRemoved()
    {
        remainingTiles--;
        Debug.Log($"Remaining Tiles: {remainingTiles}");
        if (remainingTiles <= 0)
        {
            LevelCompleted?.Invoke();
        }
    }

    public void ShuffleBoard()
    {
        List<Tile> aliveTiles = new();

        foreach (Tile tile in spawnedTiles)
        {
            if (tile != null && tile.gameObject.activeSelf)
                aliveTiles.Add(tile);
        }

        // Collect all current TileData
        List<TileData> data = new();

        foreach (Tile tile in aliveTiles)
        {
            data.Add(tile.Data);
        }

        // Shuffle the TileData list
        for (int i = data.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            (data[i], data[j]) = (data[j], data[i]);
        }

        // Assign shuffled data back to tiles
        for (int i = 0; i < aliveTiles.Count; i++)
        {
            aliveTiles[i].AnimateSetTileData(data[i]);
        }
    }
}