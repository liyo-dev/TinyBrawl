using Service;
using System;
using UnityEngine;

public class GoogleAdsService : MonoBehaviour
{
    private GoogleMobileAdsDemoScript googleAds;
    private void Start()
    {
        googleAds = ServiceLocator.GetService<GoogleMobileAdsDemoScript>();
    }
    public void ShowBanner()
    {
        googleAds.LoadAd();
    }

    public void ShowIntersitial(Action onAdClosedCallback)
    {
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
    }

}
