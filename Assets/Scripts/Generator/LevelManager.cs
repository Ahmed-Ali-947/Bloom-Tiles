using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private const string LevelKey = "CurrentLevel";

    public int CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CurrentLevel = PlayerPrefs.GetInt(LevelKey, 1);
    }

    public void CompleteLevel()
    {
        CurrentLevel++;

        PlayerPrefs.SetInt(LevelKey, CurrentLevel);
        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        CurrentLevel = 1;

        PlayerPrefs.SetInt(LevelKey, 1);

        PlayerPrefs.Save();
    }
}