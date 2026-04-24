#if POKI
namespace WebGLThirdPartyService.ServiceImplementation.Poki.Ads
{
    using MessagePipe;
    using ThirdPartyService.Core.AdsService.RewardedAds;
    using ThirdPartyService.Core.AdsService.Signals;
    using UnityEngine.Events;

    public class PokiRewardedAdsService : IRewardedAdsService
    {
        private const string AD_PLATFORM = "POKI";

        private UnityAction<bool> onAdComplete;
        private string            where;
        private bool              isInitialized;

        public int  GetPriority() => 10;
        public bool IsAdReady()   => this.isInitialized && !PokiUnitySDK.Instance.isShowingAd;

        public void Initialize()
        {
            PokiUnitySDK.Instance.rewardedBreakCallBack -= this.OnRewardedBreakCompleted;
            PokiUnitySDK.Instance.rewardedBreakCallBack += this.OnRewardedBreakCompleted;
            this.isInitialized = true;
        }

        public void ShowAd(UnityAction<bool> onAdComplete, string where)
        {
            this.onAdComplete = onAdComplete;
            this.where        = where;

            if (!this.IsAdReady())
            {
                this.onAdComplete?.Invoke(false);
                this.onAdComplete = null;
                return;
            }

            PokiUnitySDK.Instance.rewardedBreak();
            GlobalMessagePipe.GetPublisher<OnRewardedShowSignal>().Publish(new OnRewardedShowSignal(AD_PLATFORM, where));
        }

        private void OnRewardedBreakCompleted(bool withReward)
        {
            this.onAdComplete?.Invoke(withReward);
            this.onAdComplete = null;

            if (withReward)
                GlobalMessagePipe.GetPublisher<OnRewardedAdReceivedRewardEventSignal>().Publish(new OnRewardedAdReceivedRewardEventSignal(AD_PLATFORM, this.where, string.Empty, 1));

            GlobalMessagePipe.GetPublisher<OnRewardedAdHiddenEventSignal>().Publish(new OnRewardedAdHiddenEventSignal(AD_PLATFORM, this.where));
        }
    }
}
#endif
