#if POKI
namespace WebGLThirdPartyService.ServiceImplementation.Poki
{
    using WebGLThirdPartyService.Core.Poki;

    public class PokiSDKService : IPokiService
    {
        public void GameplayStart()       => PokiUnitySDK.Instance.gameplayStart();
        public void GameplayStop()        => PokiUnitySDK.Instance.gameplayStop();
        public void GameLoadingStart()    => PokiUnitySDK.Instance.gameLoadingStart();
        public void GameLoadingFinished() => PokiUnitySDK.Instance.gameLoadingFinished();
        public string GetLanguage()       => PokiUnitySDK.Instance.getLanguage();
        public bool IsAdBlocked()         => PokiUnitySDK.Instance.isAdBlocked();
        public void LogError(string error)=> PokiUnitySDK.Instance.logError(error);
    }
}
#endif
