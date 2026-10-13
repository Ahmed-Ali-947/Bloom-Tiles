using System;
using TMPro;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private TextMeshProUGUI levelText;
    private void Start()
    {
        levelText.text = $"Level {LevelManager.Instance.CurrentLevel}";
    }
    public void StartBtn()
    {
        MenuManager.Instance.StartGame();
    }
}