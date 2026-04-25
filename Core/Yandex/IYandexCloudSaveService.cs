#if YANDEX_GAMES
namespace WebGLThirdPartyService.Core.Yandex
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public interface IYandexCloudSaveService
    {
        string CachedCloudSaveJson { get; }
        string PendingCloudSaveJson { get; }
        DateTime LastCloudSyncAtUtc { get; }
        bool HasCachedCloudSave { get; }
        bool HasPendingCloudSave { get; }

        UniTask<YandexCloudSaveResult> SaveAsync(string cloudSaveJson, CancellationToken cancellation = default);
        UniTask<YandexCloudSaveResult> LoadAsync(bool useCacheIfUnavailable = true, CancellationToken cancellation = default);
        void ClearPendingCloudSave();
        void DeleteLocalCache();
    }
}
#endif
