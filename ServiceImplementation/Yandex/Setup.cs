#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using VContainer.Unity;
    using WebGLThirdPartyService.ServiceImplementation.Yandex.Ads;

    public class Setup : IAsyncStartable
    {
        private readonly YandexGamesSDKService          yandexGamesSDKService;
        private readonly YandexInterstitialAdsService   interstitialAdsService;
        private readonly YandexRewardedAdsService       rewardedAdsService;

        public Setup(
            YandexGamesSDKService        yandexGamesSDKService,
            YandexInterstitialAdsService interstitialAdsService,
            YandexRewardedAdsService     rewardedAdsService
        )
        {
            this.yandexGamesSDKService = yandexGamesSDKService;
            this.interstitialAdsService = interstitialAdsService;
            this.rewardedAdsService = rewardedAdsService;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            bool initialized = await this.yandexGamesSDKService.InitializeAsync(cancellation);
            if (!initialized)
                return;

            this.interstitialAdsService.Initialize();
            this.rewardedAdsService.Initialize();
            this.yandexGamesSDKService.GameReady();
        }
    }
}
#endif
