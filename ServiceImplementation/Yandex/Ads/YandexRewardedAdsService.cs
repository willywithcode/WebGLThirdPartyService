#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex.Ads
{
    using MessagePipe;
    using ThirdPartyService.Core.AdsService.RewardedAds;
    using ThirdPartyService.Core.AdsService.Signals;
    using UnityEngine.Events;

    public class YandexRewardedAdsService : IRewardedAdsService
    {
        private const string AD_PLATFORM = "YANDEX";

        private readonly YandexGamesSDKService yandexGamesSDKService;

        private UnityAction<bool> onAdComplete;
        private string            where;
        private bool              isInitialized;
        private bool              hasRewarded;

        public YandexRewardedAdsService(YandexGamesSDKService yandexGamesSDKService)
        {
            this.yandexGamesSDKService = yandexGamesSDKService;
        }

        public int GetPriority() => 10;

        public void Initialize()
        {
            this.isInitialized = this.yandexGamesSDKService.IsInitialized;
        }

        public void ShowAd(UnityAction<bool> onAdComplete, string where)
        {
            this.onAdComplete = onAdComplete;
            this.where = where;
            this.hasRewarded = false;

            if (!this.IsAdReady())
            {
                this.Complete(false, publishHiddenEvent: false);
                return;
            }

            this.yandexGamesSDKService.ShowRewardedAd(
                onRewardedCallback: this.OnRewarded,
                onCloseCallback: this.OnClosed,
                onErrorCallback: this.OnError
            );

            GlobalMessagePipe.GetPublisher<OnRewardedShowSignal>()
                .Publish(new OnRewardedShowSignal(AD_PLATFORM, where));
        }

        public bool IsAdReady() => this.isInitialized && this.yandexGamesSDKService.IsRewardedAdReady;

        private void OnRewarded()
        {
            this.hasRewarded = true;
        }

        private void OnClosed()
        {
            this.Complete(this.hasRewarded, publishHiddenEvent: true);
        }

        private void OnError(string _)
        {
            this.Complete(false, publishHiddenEvent: false);
        }

        private void Complete(bool rewarded, bool publishHiddenEvent)
        {
            this.onAdComplete?.Invoke(rewarded);
            this.onAdComplete = null;

            if (rewarded)
            {
                GlobalMessagePipe.GetPublisher<OnRewardedAdReceivedRewardEventSignal>()
                    .Publish(new OnRewardedAdReceivedRewardEventSignal(AD_PLATFORM, this.where, string.Empty, 1));
            }

            if (publishHiddenEvent)
            {
                GlobalMessagePipe.GetPublisher<OnRewardedAdHiddenEventSignal>()
                    .Publish(new OnRewardedAdHiddenEventSignal(AD_PLATFORM, this.where));
            }
        }
    }
}
#endif
