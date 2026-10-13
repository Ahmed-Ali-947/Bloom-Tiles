using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TrayManager trayManager;
    [SerializeField] private UIManager uiManager;

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentState = GameState.Playing;
    }

    private void OnEnable()
    {
        boardManager.LevelCompleted += HandleLevelCompleted;
        trayManager.GameLost += HandleGameLost;
    }

    private void OnDisable()
    {
        boardManager.LevelCompleted -= HandleLevelCompleted;
        trayManager.GameLost -= HandleGameLost;
    }

    private void HandleLevelCompleted()
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentState = GameState.Won;
        LevelManager.Instance.CompleteLevel();

        uiManager.ShowWinPanel();
    }

    private void HandleGameLost()
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentState = GameState.Lost;

        uiManager.ShowLosePanel();
    }

    public void HandleGamePaused()
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentState = GameState.paused;
    }

    public void HandleGameResumed()
    {
        if (CurrentState != GameState.paused)
            return;

        CurrentState = GameState.Playing;
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShuffleBoard()
    {
        boardManager.ShuffleBoard();
    }
}