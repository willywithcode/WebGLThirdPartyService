#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex.DI
{
    using VContainer;
    using WebGLThirdPartyService.ServiceImplementation.Yandex;
    using WebGLThirdPartyService.ServiceImplementation.Yandex.Ads;
    using WebGLThirdPartyService.ServiceImplementation.Yandex.LocalData;

    public static class YandexGamesVContainer
    {
        public static void RegisterYandexGamesServices(this IContainerBuilder builder)
        {
            builder.Register<YandexGamesSDKService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<YandexCloudStateLocalDataService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<YandexCloudSaveService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<YandexInterstitialAdsService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<YandexRewardedAdsService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<Setup>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif
