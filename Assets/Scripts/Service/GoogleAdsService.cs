using Service;
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
}
