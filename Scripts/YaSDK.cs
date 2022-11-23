using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace YandexSDK
{
    public class YaSDK : MonoBehaviour
    {

        public static YaSDK instance;

        public event Action onInterstitialShown;
        public event Action<string> onInterstitialFailed;
        public event Action<int> onRewardedAdOpened;
        public static event Action<string> onRewardedAdReward;
        public static event Action<int> onRewardedAdClosed;
        public static event Action<int> onRewardedAdError;

        public Queue<int> rewardedAdPlacementsAsInt = new Queue<int>();
        public Queue<string> rewardedAdsPlacements = new Queue<string>();



        [SerializeField] private int secondTillNextInterstitial = 180;

        [DllImport("__Internal")] private static extern void ShowFullscreenAd();
        [DllImport("__Internal")] private static extern void OpenRateUs();
        [DllImport("__Internal")] private static extern int ShowRewardedAd(string placement);

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

        private void Start()
        {

        }
        public void ShowInterstitial()
        {
            if (!isInterstitialReady)
            {
                return;
            }
            AudioListener.pause =true;

            Time.timeScale = 0;
            ShowFullscreenAd();
            isInterstitialReady = false;

        }

        public void ShowRewarded(string placement)
        {
            rewardedAdPlacementsAsInt.Enqueue(ShowRewardedAd(placement));
            rewardedAdsPlacements.Enqueue(placement);
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
            if (placement == rewardedAdPlacementsAsInt.Dequeue())
            {
                if (onRewardedAdReward != null)
                {
                    onRewardedAdReward?.Invoke(rewardedAdsPlacements.Dequeue());
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
            rewardedAdsPlacements.Clear();
            rewardedAdPlacementsAsInt.Clear();
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