using UnityEngine;
using System;
using GoogleMobileAds.Api;

namespace LaserCrush.ThirdParty
{
    public class AdaptiveBanner : MonoBehaviour
    {
        private Action<float> m_BannerOnAction;
        public event Action<float> BannerOnAction
        {
            add => m_BannerOnAction += value;
            remove => m_BannerOnAction -= value;
        }

        private BannerView m_BannerView;

        private readonly string m_TestAdUnitID = "ca-app-pub-3940256099942544/9214589741";
#if UNITY_ANDROID
        private readonly string m_AdUnitID = "ca-app-pub-2303943085632745/9263589206";
#elif UNITY_IPHONE
  private readonly string m_AdUnitID = "ca-app-pub-3940256099942544/2934735716";
#else
  private readonly string m_AdUnitID = "unused";
#endif

        public void Init()
        {
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus status) =>
            {
                RequestBanner();
            });
        }

        private void RequestBanner()
        {
            // Clean up banner ad before creating a new one.
            if (m_BannerView != null)
                m_BannerView.Destroy();

            AdSize adSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

            m_BannerView = new BannerView(m_TestAdUnitID, adSize, AdPosition.Bottom);


            // Register for ad events.
            m_BannerView.OnBannerAdLoaded += OnBannerAdLoaded;
            m_BannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;

            // Load a banner ad.
            m_BannerView.LoadAd(new AdRequest());

            m_BannerOnAction?.Invoke(m_BannerView.GetHeightInPixels());
        }

        #region Banner callback handlers

        private void OnBannerAdLoaded()
        {
            Debug.Log($"Ad Height: {m_BannerView.GetHeightInPixels()}, width: {m_BannerView.GetWidthInPixels()}");
        }

        private void OnBannerAdLoadFailed(LoadAdError error)
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
        }

        #endregion

        private void OnDestroy()
            => m_BannerOnAction = null;
    }
}