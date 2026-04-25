#if YANDEX_GAMES
namespace WebGLThirdPartyService.Core.Yandex
{
    using System;
    using GameFoundation.Scripts.LocalData.Interfaces;
    using WebGLThirdPartyService.ServiceImplementation.Yandex.LocalData;

    public interface IYandexLocalDataService : ILocalDataService<YandexLocalData>
    {
        bool HasCachedCloudSave { get; }
        bool HasPendingCloudSave { get; }

        void SetCachedCloudSave(string cloudSaveJson);
        void SetPendingCloudSave(string cloudSaveJson);
        void ClearPendingCloudSave();
        void MarkCloudSaveSynchronized(string cloudSaveJson, DateTime syncedAtUtc);
        void MarkAuthorizationState(bool isAuthorized, bool hasPersonalProfilePermission);
        void Reload();
    }
}
#endif
