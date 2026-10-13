using UnityEngine;
using UnityEngine.UI;

public class TrayTile : MonoBehaviour
{
    [SerializeField] private Image flowerImage;

    private TileData tileData;

    public TileData Data => tileData;

    public void Initialize(TileData data)
    {
        tileData = data;
        flowerImage.sprite = data.Sprite;
    }
}