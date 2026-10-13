using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    public event System.Action OnRewardedAdLoaded;
    [Header("Ad Unit IDs")]
    [SerializeField] private string bannerAdUnitId;
    [SerializeField] private string rewardedAdUnitId;

    private BannerView bannerView;
    private RewardedAd rewardedAd;
    private TMP_Text debugText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MobileAds.Initialize((InitializationStatus status) =>
        {
            Debug.Log("AdMob Initialized");

            LoadBanner();
            LoadRewardedAd();
        });
    }

    private void CreateDebugText()
    {
        if (debugText != null)
            return;

        Canvas canvas = FindFirstObjectByType<Canvas>();

        GameObject go = new GameObject("DebugText");
        go.transform.SetParent(canvas.transform, false);

        debugText = go.AddComponent<TextMeshProUGUI>();
        debugText.fontSize = 40;
        debugText.color = Color.white;

        RectTransform rt = debugText.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -30);
        rt.sizeDelta = new Vector2(800, 150);
    }
    private void LoadBanner()
    {
        if (bannerView != null)
            return;

        bannerView = new BannerView(
            bannerAdUnitId,
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth),
            AdPosition.Bottom);

        AdRequest request = new AdRequest();

        bannerView.OnBannerAdLoaded += () =>
        {
            // CreateDebugText();
            // debugText.text = "Banner Loaded";
            // debugText1.text = "Banner Loaded";

            Debug.Log("Banner Loaded");
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            // CreateDebugText();
            // debugText.text = error.ToString();
            // debugText1.text = error.ToString();
            Debug.LogError(error);
        };

        bannerView.LoadAd(request);

    }

    public void ShowBanner()
    {
        bannerView?.Show();
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    private void OnDestroy()
    {
        bannerView?.Destroy();
    }

    private void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        AdRequest request = new AdRequest();

        RewardedAd.Load(
            rewardedAdUnitId,
            request,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.Log($"Rewarded failed to load: {error}");
                    if (error != null)
                    {
                        // CreateDebugText();
                        // debugText.text = error.ToString();
                        // debugText2.text = error.ToString();

                        return;
                    }
                    return;
                }

                // CreateDebugText();
                // debugText.text = "Rewarded loaded.";
                // debugText2.text = "Rewarded loaded.";


                Debug.Log("Rewarded loaded.");
                rewardedAd = ad;

                OnRewardedAdLoaded?.Invoke();
                RegisterRewardedEvents(ad);
            });
    }

    private void RegisterRewardedEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded closed.");

            rewardedAd.Destroy();
            rewardedAd = null;

            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log(error);

            rewardedAd.Destroy();
            rewardedAd = null;

            LoadRewardedAd();
        };
    }

    public void ShowRewardedAd(System.Action onRewardEarned)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Reward earned: {reward.Amount}");

                onRewardEarned?.Invoke();
            });
        }
        else
        {
            Debug.Log("Rewarded ad not ready.");
        }
    }

    public bool IsRewardedAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        LoadBanner();

        if (rewardedAd == null)
        {
            LoadRewardedAd();
        }
    }
}