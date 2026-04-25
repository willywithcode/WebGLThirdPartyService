#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using WebGLThirdPartyService.Core.Yandex;

    public class YandexCloudSaveService : IYandexCloudSaveService
    {
        private readonly IYandexGamesService yandexGamesService;
        private readonly IYandexLocalDataService yandexLocalDataService;

        public YandexCloudSaveService(IYandexGamesService yandexGamesService, IYandexLocalDataService yandexLocalDataService)
        {
            this.yandexGamesService = yandexGamesService;
            this.yandexLocalDataService = yandexLocalDataService;
        }

        public string CachedCloudSaveJson => this.yandexLocalDataService.Data.CachedCloudSaveJson;

        public string PendingCloudSaveJson => this.yandexLocalDataService.Data.PendingCloudSaveJson;

        public DateTime LastCloudSyncAtUtc => this.yandexLocalDataService.Data.LastCloudSyncAtUtcTicks <= 0
            ? DateTime.MinValue
            : new DateTime(this.yandexLocalDataService.Data.LastCloudSyncAtUtcTicks, DateTimeKind.Utc);

        public bool HasCachedCloudSave => this.yandexLocalDataService.HasCachedCloudSave;

        public bool HasPendingCloudSave => this.yandexLocalDataService.HasPendingCloudSave;

        public async UniTask<YandexCloudSaveResult> SaveAsync(string cloudSaveJson, CancellationToken cancellation = default)
        {
            string sanitizedJson = SanitizeJson(cloudSaveJson);
            this.yandexLocalDataService.SetPendingCloudSave(sanitizedJson);
            this.UpdateAuthorizationSnapshot();

            if (!this.yandexGamesService.IsRunningOnYandex || !this.yandexGamesService.IsInitialized)
            {
                return new YandexCloudSaveResult(
                    isSuccess: false,
                    cloudSaveJson: sanitizedJson,
                    usedLocalCache: true,
                    errorMessage: "Yandex Games runtime is unavailable."
                );
            }

            var completionSource = new UniTaskCompletionSource<YandexCloudSaveResult>();

            using (cancellation.Register(() => completionSource.TrySetCanceled(cancellation)))
            {
                this.yandexGamesService.SetCloudSaveData(
                    sanitizedJson,
                    onSuccessCallback: () =>
                    {
                        this.yandexLocalDataService.MarkCloudSaveSynchronized(sanitizedJson, DateTime.UtcNow);
                        completionSource.TrySetResult(new YandexCloudSaveResult(true, sanitizedJson, false, string.Empty));
                    },
                    onErrorCallback: errorMessage =>
                    {
                        completionSource.TrySetResult(new YandexCloudSaveResult(false, sanitizedJson, false, errorMessage));
                    }
                );

                return await completionSource.Task;
            }
        }

        public async UniTask<YandexCloudSaveResult> LoadAsync(bool useCacheIfUnavailable = true, CancellationToken cancellation = default)
        {
            this.UpdateAuthorizationSnapshot();

            if (!this.yandexGamesService.IsRunningOnYandex || !this.yandexGamesService.IsInitialized)
            {
                if (useCacheIfUnavailable && this.HasCachedCloudSave)
                {
                    return new YandexCloudSaveResult(true, this.CachedCloudSaveJson, true, string.Empty);
                }

                return new YandexCloudSaveResult(false, string.Empty, false, "Yandex Games runtime is unavailable.");
            }

            var completionSource = new UniTaskCompletionSource<YandexCloudSaveResult>();

            using (cancellation.Register(() => completionSource.TrySetCanceled(cancellation)))
            {
                this.yandexGamesService.GetCloudSaveData(
                    onSuccessCallback: cloudSaveJson =>
                    {
                        string sanitizedJson = SanitizeJson(cloudSaveJson);
                        this.yandexLocalDataService.MarkCloudSaveSynchronized(sanitizedJson, DateTime.UtcNow);
                        completionSource.TrySetResult(new YandexCloudSaveResult(true, sanitizedJson, false, string.Empty));
                    },
                    onErrorCallback: errorMessage =>
                    {
                        if (useCacheIfUnavailable && this.HasCachedCloudSave)
                        {
                            completionSource.TrySetResult(new YandexCloudSaveResult(true, this.CachedCloudSaveJson, true, errorMessage));
                            return;
                        }

                        completionSource.TrySetResult(new YandexCloudSaveResult(false, string.Empty, false, errorMessage));
                    }
                );

                return await completionSource.Task;
            }
        }

        public void ClearPendingCloudSave()
        {
            this.yandexLocalDataService.ClearPendingCloudSave();
        }

        public void DeleteLocalCache()
        {
            this.yandexLocalDataService.DeleteData();
        }

        private void UpdateAuthorizationSnapshot()
        {
            this.yandexLocalDataService.MarkAuthorizationState(
                this.yandexGamesService.IsAuthorized,
                this.yandexGamesService.HasPersonalProfileDataPermission
            );
        }

        private static string SanitizeJson(string cloudSaveJson)
        {
            return string.IsNullOrWhiteSpace(cloudSaveJson) ? "{}" : cloudSaveJson;
        }
    }
}
#endif
