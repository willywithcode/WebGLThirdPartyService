#if POKI
namespace WebGLThirdPartyService.ServiceImplementation.Poki.DI
{
    using WebGLThirdPartyService.ServiceImplementation.Poki;
    using WebGLThirdPartyService.ServiceImplementation.Poki.Ads;
    using VContainer;

    public static class PokiVContainer
    {
        public static void RegisterPokiServices(this IContainerBuilder builder)
        {
            builder.Register<PokiSDKService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PokiInterstitialAdsService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PokiRewardedAdsService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<Setup>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif
