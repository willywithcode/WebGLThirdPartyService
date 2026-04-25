#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex.LocalData
{
    using System;
    using GameFoundation.Scripts.LocalData.Interfaces;

    [Serializable]
    public class YandexLocalData : ILocalData
    {
        public string CachedCloudSaveJson = string.Empty;
        public string PendingCloudSaveJson = string.Empty;
        public string LastSyncedCloudSaveJson = string.Empty;
        public string LastCloudErrorMessage = string.Empty;
        public long LastCloudSyncAtUtcTicks;
        public bool IsAuthorized;
        public bool HasPersonalProfilePermission;

        public string GetKey() => this.GetType().ToString();

        public void Reset()
        {
            this.CachedCloudSaveJson = string.Empty;
            this.PendingCloudSaveJson = string.Empty;
            this.LastSyncedCloudSaveJson = string.Empty;
            this.LastCloudErrorMessage = string.Empty;
            this.LastCloudSyncAtUtcTicks = 0;
            this.IsAuthorized = false;
            this.HasPersonalProfilePermission = false;
        }
    }
}
#endif
