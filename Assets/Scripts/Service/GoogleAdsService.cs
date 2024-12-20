using Service;
using System;
using UnityEngine;

public class GoogleAdsService : MonoBehaviour
{
    private GoogleMobileAdsDemoScript googleAds;
    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        googleAds = ServiceLocator.GetService<GoogleMobileAdsDemoScript>();
#endif
    }
    public void ShowBanner()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        googleAds.LoadAd();
#endif
    }

    public void ShowIntersitialTest()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        googleAds.LoadInterstitialAd();

        if (googleAds.InterstitialAd != null)
        {
            googleAds.InterstitialAd.Show();

            googleAds.InterstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad closed.");
            };
        }
        else
        {
            Debug.LogWarning("Interstitial ad not ready. Executing callback.");
        }
#endif
    }

    public void ShowIntersitial(Action onAdClosedCallback)
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        googleAds.LoadInterstitialAd();

        if (googleAds.InterstitialAd != null)
        {
            googleAds.InterstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad closed.");
                onAdClosedCallback?.Invoke();
            };

            googleAds.InterstitialAd.Show();
        }
        else
        {
            // Si no hay anuncio cargado, ejecuta el callback directamente
            Debug.LogWarning("Interstitial ad not ready. Executing callback.");
            onAdClosedCallback?.Invoke();
        }
#endif
    }

}
