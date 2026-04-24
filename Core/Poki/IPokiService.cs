namespace WebGLThirdPartyService.Core.Poki
{
    public interface IPokiService
    {
        void GameplayStart();
        void GameplayStop();
        void GameLoadingStart();
        void GameLoadingFinished();
        string GetLanguage();
        bool IsAdBlocked();
        void LogError(string error);
    }
}
