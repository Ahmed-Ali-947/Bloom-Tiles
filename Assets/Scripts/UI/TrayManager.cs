using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public class TrayManager : MonoBehaviour
{
    public event Action TileAddedToTray;
    public event Action GameLost;
    [SerializeField] private TrayTile trayTilePrefab;
    [SerializeField] private FlyingTile flyingTilePrefab;
    [SerializeField] private RectTransform[] slots;

    private readonly List<TrayTile> trayTiles = new();
    public void AddTile(Tile boardTile)
    {
        if (trayTiles.Count >= slots.Length)
            return;

        boardTile.GetComponent<Collider2D>().enabled = false;

        TrayTile trayTile =
        Instantiate(trayTilePrefab, transform);

        trayTile.Initialize(boardTile.Data);

        trayTile.transform.localScale = Vector3.zero;

        int insertIndex = FindInsertionIndex(boardTile.Data);

        RectTransform rect = trayTile.GetComponent<RectTransform>();

        // Put it in the correct slot immediately
        rect.position = slots[insertIndex].position;

        trayTiles.Insert(insertIndex, trayTile);

        ArrangeTray();

        boardTile.RemoveTile();

        Sequence pop = DOTween.Sequence();

        pop.SetLink(trayTile.gameObject);

        pop.Append(
            trayTile.transform.DOScale(1.15f, 0.12f));

        pop.Append(
            trayTile.transform.DOScale(1f, 0.08f));

        pop.OnComplete(() =>
        {
            TileAddedToTray?.Invoke();
            CheckMatches();
        });
    }
    private int FindInsertionIndex(TileData data)
    {
        trayTiles.RemoveAll(t => t == null);

        for (int i = trayTiles.Count - 1; i >= 0; i--)
        {
            if (trayTiles[i] != null &&
                trayTiles[i].Data == data)
            {
                return i + 1;
            }
        }

        return trayTiles.Count;
    }

    private void ArrangeTray(bool animate = true)
    {
        trayTiles.RemoveAll(t => t == null);

        for (int i = 0; i < trayTiles.Count; i++)
        {
            TrayTile tile = trayTiles[i];

            RectTransform rect =
                tile.GetComponent<RectTransform>();

            Vector3 target =
                slots[i].position;

            rect.DOKill();

            if (animate)
            {
                rect.DOMove(target, 0.18f)
                    .SetEase(Ease.OutQuad);
            }
            else
            {
                rect.position = target;
            }
        }
    }
    private void CheckMatches()
    {
        trayTiles.RemoveAll(t => t == null);

        Dictionary<TileData, List<TrayTile>> groups = new();

        foreach (TrayTile tile in trayTiles)
        {
            if (!groups.TryGetValue(tile.Data, out var list))
            {
                list = new List<TrayTile>();
                groups.Add(tile.Data, list);
            }

            list.Add(tile);
        }

        foreach (var pair in groups)
        {
            if (pair.Value.Count >= 3)
            {
                RemoveMatch(pair.Value.GetRange(0, 3));
                return;
            }
        }

        CheckLose();
    }

    private void RemoveMatch(List<TrayTile> matchedTiles)
    {
        foreach (TrayTile tile in matchedTiles)
        {
            tile.transform.DOKill();
        }

        Sequence sequence = DOTween.Sequence();

        foreach (TrayTile tile in matchedTiles)
        {
            sequence.Join(
                tile.transform.DOScale(1.15f, 0.08f));
        }

        sequence.AppendInterval(0.08f);

        foreach (TrayTile tile in matchedTiles)
        {
            sequence.Join(
                tile.transform.DOScale(0f, 0.15f));
        }

        sequence.OnComplete(() =>
        {
            foreach (TrayTile tile in matchedTiles)
            {
                trayTiles.Remove(tile);
                Destroy(tile.gameObject);
            }

            ArrangeTray();

            CheckMatches();
        });
    }

    private void CheckLose()
    {
        trayTiles.RemoveAll(t => t == null);

        if (trayTiles.Count >= slots.Length)
        {
            GameLost?.Invoke();
        }
    }

}