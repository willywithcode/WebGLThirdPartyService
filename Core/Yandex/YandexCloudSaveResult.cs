#if YANDEX_GAMES
namespace WebGLThirdPartyService.Core.Yandex
{
    public readonly struct YandexCloudSaveResult
    {
        public YandexCloudSaveResult(bool isSuccess, string cloudSaveJson, bool usedLocalCache, string errorMessage)
        {
            this.IsSuccess = isSuccess;
            this.CloudSaveJson = cloudSaveJson;
            this.UsedLocalCache = usedLocalCache;
            this.ErrorMessage = errorMessage ?? string.Empty;
        }

        public bool IsSuccess { get; }
        public string CloudSaveJson { get; }
        public bool UsedLocalCache { get; }
        public string ErrorMessage { get; }
    }
}
#endif
