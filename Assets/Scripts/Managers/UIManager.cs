using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private ParticleSystem onClickParticle;
    [SerializeField] private UnityEngine.UI.Button rewardButton;

    private void Awake()
    {
        HideAllPanels();
    }

    private void Start()
    {
        levelText.text = $"Level {LevelManager.Instance.CurrentLevel}";
        AdsManager.Instance.OnRewardedAdLoaded += UpdateRewardButtonState;
    }
    private void UpdateRewardButtonState()
    {
        if (AdsManager.Instance.IsRewardedAdReady() == true)
        {
            rewardButton.interactable = true;
        }
        else
        {
            rewardButton.interactable = false;
        }
    }
    // public void UpdateTestText(string message)
    // {
    //     testText.text = message;
    // }
    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
    }
    public void ShowMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        GameManager.Instance.HandleGamePaused();
    }

    public void HideAllPanels()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        if (GameManager.Instance != null)
            GameManager.Instance.HandleGameResumed();
    }

    public void NextLevelBtn()
    {
        GameManager.Instance.NextLevel();
    }

    public void RestartLevelBtn()
    {
        GameManager.Instance.RestartLevel();
    }

    public void MainMenuBtn()
    {
        GameManager.Instance.MainMenu();
    }

    public void ShuffleBoardBtn()
    {
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            GameManager.Instance.ShuffleBoard();
        });
    }

    public void OnClickParticle(Vector3 position)
    {
        Instantiate(onClickParticle, new Vector3(position.x, position.y, 0), Quaternion.identity);
    }
}