#if POKI
namespace WebGLThirdPartyService.ServiceImplementation.Poki
{
    using WebGLThirdPartyService.ServiceImplementation.Poki.Ads;
    using VContainer.Unity;

    public class Setup : IInitializable
    {
        private readonly PokiSDKService             pokiSDKService;
        private readonly PokiInterstitialAdsService interstitialAdsService;
        private readonly PokiRewardedAdsService     rewardedAdsService;

        public Setup(
            PokiSDKService             pokiSDKService,
            PokiInterstitialAdsService interstitialAdsService,
            PokiRewardedAdsService     rewardedAdsService
        )
        {
            this.pokiSDKService         = pokiSDKService;
            this.interstitialAdsService = interstitialAdsService;
            this.rewardedAdsService     = rewardedAdsService;
        }

        public void Initialize()
        {
            PokiUnitySDK.Instance.sdkInitializedCallback -= this.OnSdkInitialized;
            PokiUnitySDK.Instance.sdkInitializedCallback += this.OnSdkInitialized;
            PokiUnitySDK.Instance.gameLoadingStart();
            PokiUnitySDK.Instance.init();
        }

        private void OnSdkInitialized()
        {
            this.interstitialAdsService.Initialize();
            this.rewardedAdsService.Initialize();
            this.pokiSDKService.GameLoadingFinished();
        }
    }
}
#endif
