using UnityEngine;
using UnityEngine.UI;

public class FlyingTile : MonoBehaviour
{
    [SerializeField] private Image flowerImage;
    public void Initialize(TileData tileData)
    {
        flowerImage.sprite = tileData.Sprite;
    }
}