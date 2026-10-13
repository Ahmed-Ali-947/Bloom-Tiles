using UnityEngine;
[System.Serializable]
public class LayerSettings
{
    [SerializeField] private bool horizontalOffset;
    [SerializeField] private bool verticalOffset;
    public bool HorizontalOffset => horizontalOffset;
    public bool VerticalOffset => verticalOffset;

    public LayerSettings(bool horizontal, bool vertical)
    {
        horizontalOffset = horizontal;
        verticalOffset = vertical;
    }
}