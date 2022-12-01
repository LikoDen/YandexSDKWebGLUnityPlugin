using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEditor;
namespace YandexSDK
{
    public enum Platform
    {
        phone,
        desktop
    }
    public class YaSDK : MonoBehaviour
    {
        public static YaSDK instance;
        public delegate void onPlayerAuthenticatedHandler();
        public static event onPlayerAuthenticatedHandler onPlayerAuthenticated;
        public delegate void onGetPlayerDataHandler(string item);
        public static event onGetPlayerDataHandler onGetPlayerData;
        public event Action onInterstitialShown;
        public event Action<string> onInterstitialFailed;
        public event Action<int> onRewardedAdOpened;
        public static event Action<string> onRewardedAdReward;
        public static event Action<int> onRewardedAdClosed;
        public static event Action<int> onRewardedAdError;
        public int rewardedAdPlacementAsInt = 0;
        public string rewardedAdPlacement = string.Empty;
        [SerializeField] private int secondTillNextInterstitial = 180;
        [DllImport("__Internal")] private static extern void Authenticate();
        [DllImport("__Internal")] private static extern void SetPlayerData(string data);
        [DllImport("__Internal")] private static extern void GetPlayerData();
        [DllImport("__Internal")] private static extern void ShowFullscreenAd();
        [DllImport("__Internal")] private static extern void OpenRateUs();
        [DllImport("__Internal")] private static extern int ShowRewardedAd(string placement);
        public Platform currentPlatform;
        private int currentSecondsTillNextInterstitial;
        private bool isInterstitialReady = false;

        public void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            currentSecondsTillNextInterstitial = secondTillNextInterstitial;
            StartCoroutine(CountTillNextInterstitial());
        }

        public void AuthenticateUser()
        {
            Authenticate();
        }
        public void OnPlayerAuthenticated()
        {
            onPlayerAuthenticated?.Invoke();
        }
        public void SetSave<T>(T saveStateClass)
        {
            string dataStr = JsonUtility.ToJson(saveStateClass);
            SetPlayerData(dataStr);
        }
        public void GetSave()
        {
            GetPlayerData();
        }
        public void OnGetPlayerData(string dataStr)
        {
            if (!dataStr.Contains("none"))
            {
                onGetPlayerData?.Invoke(dataStr);
            }
            else
            {
                onGetPlayerData?.Invoke(string.Empty);
            }
        }
        public void OnGetPlayerPlatform(string p)
        {
            switch (p)
            {
                case "phone":
                    currentPlatform = Platform.phone;
                    break;
                case "desktop":
                    currentPlatform = Platform.desktop;
                    break;
            }
        }
        public void ShowInterstitial()
        {
            if (!isInterstitialReady)
            {
                return;
            }
            AudioListener.pause = true;
            isInterstitialReady = false;
            Time.timeScale = 0;
            ShowFullscreenAd();


        }

        public void ShowRewarded(string placement)
        {
            rewardedAdPlacementAsInt = (ShowRewardedAd(placement));
            rewardedAdPlacement = (placement);
            AudioListener.pause = true;
        }

        public void OnInterstitialShown()
        {
            if (onInterstitialShown != null)
            {
                onInterstitialShown();
            }
            AudioListener.pause = false;
            Time.timeScale = 1;
            StartCoroutine(CountTillNextInterstitial());
        }

        public void OnInterstitialError(string error)
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            StartCoroutine(CountTillNextInterstitial());
            if (onInterstitialFailed != null)
            {
                onInterstitialFailed(error);
            }
        }

        public void OnRewardedOpen(int placement)
        {
            Time.timeScale = 0;
            if (onRewardedAdOpened != null)
            {
                onRewardedAdOpened(placement);
            }
        }

        public void OnRewarded(int placement)
        {
            if (placement == rewardedAdPlacementAsInt)
            {
                if (onRewardedAdReward != null)
                {
                    onRewardedAdReward?.Invoke(rewardedAdPlacement);
                }
            }
        }

        public void OnRewardedClose(int placement)
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            if (onRewardedAdClosed != null)
            {
                onRewardedAdClosed(placement);
            }
        }

        public void OnRewardedError(int placement)
        {

            Time.timeScale = 1;
            AudioListener.pause = false;
            if (onRewardedAdError != null)
            {
                onRewardedAdError(placement);
            }
        }
        public void OpenRateUsWindow()
        {
            OpenRateUs();
        }
        private IEnumerator<WaitForSeconds> CountTillNextInterstitial()
        {
            while (currentSecondsTillNextInterstitial > 0)
            {

                yield return new WaitForSeconds(1);
                currentSecondsTillNextInterstitial--;
            }
            isInterstitialReady = true;
            currentSecondsTillNextInterstitial = secondTillNextInterstitial;
        }

    }
}
