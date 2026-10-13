using UnityEngine;

[CreateAssetMenu(menuName = "Bloom Tiles/Tile")]
public class TileData : ScriptableObject
{
    [SerializeField] private int id;

    [SerializeField] private Sprite sprite;

    public int ID => id;

    public Sprite Sprite => sprite;
}