#if POKI
namespace WebGLThirdPartyService.ServiceImplementation.Poki.Ads
{
    using MessagePipe;
    using ThirdPartyService.Core.AdsService.InterstitialsAds;
    using ThirdPartyService.Core.AdsService.Signals;
    using UnityEngine.Events;

    public class PokiInterstitialAdsService : IInterstitialAdsService
    {
        private const string AD_PLATFORM = "POKI";

        private UnityAction onAdClosed;
        private UnityAction onAdFailedToShow;
        private string      where;
        private bool        isInitialized;

        public int  GetPriority()   => 10;
        public bool IsInitialized() => this.isInitialized;

        public void Initialize()
        {
            PokiUnitySDK.Instance.commercialBreakCallBack -= this.OnCommercialBreakCompleted;
            PokiUnitySDK.Instance.commercialBreakCallBack += this.OnCommercialBreakCompleted;
            this.isInitialized = true;
        }

        public void ShowInterstitial(string where, UnityAction onAdClosed = null, UnityAction onAdFailedToShow = null)
        {
            this.where             = where;
            this.onAdClosed        = onAdClosed;
            this.onAdFailedToShow  = onAdFailedToShow;

            if (!this.IsInterstitialReady())
            {
                this.onAdFailedToShow?.Invoke();
                this.onAdFailedToShow = null;
                return;
            }

            PokiUnitySDK.Instance.commercialBreak();
            GlobalMessagePipe.GetPublisher<OnInterstitialShowSignal>().Publish(new OnInterstitialShowSignal(AD_PLATFORM, where));
        }

        public bool IsInterstitialReady() => this.isInitialized && !PokiUnitySDK.Instance.isShowingAd && !PokiUnitySDK.Instance.adblocked;

        private void OnCommercialBreakCompleted()
        {
            this.onAdClosed?.Invoke();
            this.onAdClosed = null;
            GlobalMessagePipe.GetPublisher<OnInterstitialAdHiddenEventSignal>().Publish(new OnInterstitialAdHiddenEventSignal(AD_PLATFORM, this.where));
        }
    }
}
#endif
