using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
public class Tile : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer flowerRenderer;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private BoxCollider2D tileCollider;

    public Collider2D Collider => tileCollider;
    private readonly List<Tile> coveredBy = new();
    private readonly List<Tile> covering = new();

    private TileData tileData;

    private int layer;

    public int Layer => layer;

    public bool IsBlocked => coveredBy.Count > 0;

    public IReadOnlyList<Tile> CoveredBy => coveredBy;

    public IReadOnlyList<Tile> Covering => covering;

    public TileData Data => tileData;
    public int SortingOrder => sortingGroup.sortingOrder;
    private Vector3 flowerOriginalScale;
    private void Awake()
    {
        if (tileCollider == null)
            tileCollider = GetComponent<BoxCollider2D>();

        flowerOriginalScale = flowerRenderer.transform.localScale;
    }
    public void Initialize(TileData data, int tileLayer)
    {
        tileData = data;
        layer = tileLayer;

        flowerRenderer.sprite = data.Sprite;
    }
    public void SetTileData(TileData data)
    {
        tileData = data;
        flowerRenderer.sprite = data.Sprite;
    }

    public void AnimateSetTileData(TileData data)
    {
        flowerRenderer.transform.DOKill();
        flowerRenderer.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Join(
            flowerRenderer.transform
                .DOScale(Vector3.zero, 0.15f)
                .SetEase(Ease.InBack));

        seq.Join(
            flowerRenderer
                .DOFade(0f, 0.12f));

        seq.AppendCallback(() =>
        {
            tileData = data;
            flowerRenderer.sprite = data.Sprite;
        });

        seq.Join(
            flowerRenderer.transform
                .DOScale(flowerOriginalScale, 0.2f)
                .SetEase(Ease.OutBack));

        seq.Join(
            flowerRenderer
                .DOFade(1f, 0.2f));
    }
    public void SetSortingOrder(int order)
    {
        sortingGroup.sortingOrder = order;
    }

    public void AddBlocker(Tile blocker)
    {
        if (coveredBy.Contains(blocker))
            return;

        coveredBy.Add(blocker);
    }

    public void AddCoveredTile(Tile tile)
    {
        if (covering.Contains(tile))
            return;

        covering.Add(tile);
    }

    private void RemoveBlocker(Tile blocker)
    {
        if (!coveredBy.Remove(blocker))
            return;

        RefreshState();
    }

    public void RemoveTile()
    {
        foreach (Tile tile in covering)
            tile.RemoveBlocker(this);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void RefreshState()
    {
        Color color = IsBlocked
            ? Color.gray
            : Color.white;

        bodyRenderer.color = color;
        flowerRenderer.color = color;
    }

    public void Select(BoardManager boardManager)
    {
        if (IsBlocked)
            return;

        boardManager.HandleTileSelected(this);
    }

}