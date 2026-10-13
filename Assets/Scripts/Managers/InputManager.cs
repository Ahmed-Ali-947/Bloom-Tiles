using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private UIManager uiManager;
    private BloomInputActions input;

    private void Awake()
    {
        input = new BloomInputActions();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Tap.performed += OnTap;
    }

    private void OnDisable()
    {
        input.Player.Tap.performed -= OnTap;

        input.Disable();
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        uiManager.OnClickParticle(mainCamera.ScreenToWorldPoint(input.Player.PointerPosition.ReadValue<Vector2>()));
        if (gameManager.CurrentState != GameState.Playing)
            return;

        Vector2 screenPosition =
            input.Player.PointerPosition.ReadValue<Vector2>();

        Vector2 worldPosition =
            mainCamera.ScreenToWorldPoint(screenPosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition, tileLayer);

        Tile topTile = null;
        int highestOrder = int.MinValue;

        foreach (Collider2D hit in hits)
        {
            Tile tile = hit.GetComponent<Tile>();

            if (tile == null)
                continue;

            if (tile.SortingOrder > highestOrder)
            {
                highestOrder = tile.SortingOrder;
                topTile = tile;
            }
        }

        if (topTile != null)
        {
            topTile.Select(boardManager);
        }
    }
}