using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;

public class GoogleAdMob : MonoBehaviour
{
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private readonly string m_AdUnitID = "ca-app-pub-2303943085632745/9263589806";
#elif UNITY_IPHONE
  private readonly string m_AdUnitID = "ca-app-pub-3940256099942544/2934735716";
#else
  private readonly string m_AdUnitID = "unused";
#endif

    private BannerView m_BannerView;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });
        CreateBannerView();
        LoadAd();
    }

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (m_BannerView is not null)
        {
            m_BannerView.Destroy();
            m_BannerView = null;
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        //베너 사이즈 자동 설정 
        m_BannerView = new BannerView(m_AdUnitID, adaptiveSize, AdPosition.Bottom);


        //베너 사이즈 사용자가 직접 지정
        //_bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);

        //포지션 지정 coordinate (0,50)
        //_bannerView = new BannerView(_adUnitId, AdSize.Banner, 0, 50);

        //베너 사이즈 지정
        //AdSize adSize = new AdSize(250, 250);
        //_bannerView = new BannerView(_adUnitId, adSize, AdPosition.Bottom);
    }


    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (m_BannerView is null) CreateBannerView();

        // create our request used to load the ad.
        AdRequest adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        m_BannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerView()
    {
        if (m_BannerView is not null)
        {
            Debug.Log("Destroying banner view.");
            m_BannerView.Destroy();
            m_BannerView = null;
        }
    }
}
