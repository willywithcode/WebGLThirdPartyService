#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex.LocalData
{
    using System;
    using WebGLThirdPartyService.Core.Yandex;

    public class YandexCloudStateLocalDataService : YandexLocalDataService<YandexLocalData>, IYandexLocalDataService
    {
        public bool HasCachedCloudSave => !string.IsNullOrEmpty(this.Data.CachedCloudSaveJson);

        public bool HasPendingCloudSave => !string.IsNullOrEmpty(this.Data.PendingCloudSaveJson);

        public void SetCachedCloudSave(string cloudSaveJson)
        {
            this.Data.CachedCloudSaveJson = SanitizeJson(cloudSaveJson);
            this.Save();
        }

        public void SetPendingCloudSave(string cloudSaveJson)
        {
            this.Data.PendingCloudSaveJson = SanitizeJson(cloudSaveJson);
            this.Save();
        }

        public void ClearPendingCloudSave()
        {
            this.Data.PendingCloudSaveJson = string.Empty;
            this.Save();
        }

        public void MarkCloudSaveSynchronized(string cloudSaveJson, DateTime syncedAtUtc)
        {
            string sanitizedJson = SanitizeJson(cloudSaveJson);
            this.Data.CachedCloudSaveJson = sanitizedJson;
            this.Data.LastSyncedCloudSaveJson = sanitizedJson;
            this.Data.PendingCloudSaveJson = string.Empty;
            this.Data.LastCloudErrorMessage = string.Empty;
            this.Data.LastCloudSyncAtUtcTicks = syncedAtUtc.ToUniversalTime().Ticks;
            this.Save();
        }

        public void MarkAuthorizationState(bool isAuthorized, bool hasPersonalProfilePermission)
        {
            this.Data.IsAuthorized = isAuthorized;
            this.Data.HasPersonalProfilePermission = hasPersonalProfilePermission;
            this.Save();
        }

        public void Reload()
        {
            this.Load();
        }

        private static string SanitizeJson(string cloudSaveJson)
        {
            return string.IsNullOrWhiteSpace(cloudSaveJson) ? "{}" : cloudSaveJson;
        }
    }
}
#endif
