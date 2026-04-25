#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex.Ads
{
    using MessagePipe;
    using ThirdPartyService.Core.AdsService.InterstitialsAds;
    using ThirdPartyService.Core.AdsService.Signals;
    using UnityEngine.Events;

    public class YandexInterstitialAdsService : IInterstitialAdsService
    {
        private const string AD_PLATFORM = "YANDEX";

        private readonly YandexGamesSDKService yandexGamesSDKService;

        private UnityAction onShowFail;
        private UnityAction onShowSuccess;
        private string      where;
        private bool        isInitialized;

        public YandexInterstitialAdsService(YandexGamesSDKService yandexGamesSDKService)
        {
            this.yandexGamesSDKService = yandexGamesSDKService;
        }

        public int GetPriority() => 10;

        public bool IsInitialized() => this.isInitialized;

        public void Initialize()
        {
            this.isInitialized = this.yandexGamesSDKService.IsInitialized;
        }

        public void ShowInterstitial(string where, UnityAction onAdClosed = null, UnityAction onAdFailedToShow = null)
        {
            // AdsService currently passes callbacks in the opposite order:
            // param2 = fail callback, param3 = success callback.
            this.onShowFail = onAdClosed;
            this.onShowSuccess = onAdFailedToShow;
            this.where = where;

            if (!this.IsInterstitialReady())
            {
                this.NotifyFailedToShow();
                return;
            }

            this.yandexGamesSDKService.ShowInterstitialAd(
                onCloseCallback: this.OnInterstitialClosed,
                onErrorCallback: this.OnInterstitialError,
                onOfflineCallback: this.OnInterstitialOffline
            );

            GlobalMessagePipe.GetPublisher<OnInterstitialShowSignal>()
                .Publish(new OnInterstitialShowSignal(AD_PLATFORM, where));
        }

        public bool IsInterstitialReady() => this.isInitialized && this.yandexGamesSDKService.IsInterstitialAdReady;

        private void OnInterstitialClosed(bool wasShown)
        {
            if (!wasShown)
            {
                this.NotifyFailedToShow();
                return;
            }

            this.onShowSuccess?.Invoke();
            this.onShowSuccess = null;
            this.onShowFail = null;

            GlobalMessagePipe.GetPublisher<OnInterstitialAdHiddenEventSignal>()
                .Publish(new OnInterstitialAdHiddenEventSignal(AD_PLATFORM, this.where));
        }

        private void OnInterstitialError(string _)
        {
            this.NotifyFailedToShow();
        }

        private void OnInterstitialOffline()
        {
            this.NotifyFailedToShow();
        }

        private void NotifyFailedToShow()
        {
            this.onShowFail?.Invoke();
            this.onShowFail = null;
            this.onShowSuccess = null;
        }
    }
}
#endif
