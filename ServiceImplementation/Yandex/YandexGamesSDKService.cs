#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex
{
    using System;
    using System.Threading;
    using BananaParty.YandexGames;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using WebGLThirdPartyService.Core.Yandex;

    public class YandexGamesSDKService : IYandexGamesService
    {
        private const string SDK_UNAVAILABLE_MESSAGE = "Yandex Games SDK is available only in WebGL builds running on Yandex Games.";
        private const string SDK_NOT_INITIALIZED_MESSAGE = "Yandex Games SDK is not initialized.";

        private bool isInitialized;
        private bool isStickyAdVisible;

        public YandexGamesSDKService()
        {
            PlayerAccount.AuthorizedInBackground += this.OnAuthorizedInBackground;
        }

        public bool IsRunningOnYandex => this.IsWebGLRuntime && YandexGamesSdk.IsRunningOnYandex;

        public bool IsInitialized => this.isInitialized;

        public bool IsAuthorized => this.CanUseSdk && PlayerAccount.IsAuthorized;

        public bool HasPersonalProfileDataPermission => this.CanUseSdk && PlayerAccount.HasPersonalProfileDataPermission;

        public bool IsInterstitialAdReady => this.CanUseSdk;

        public bool IsRewardedAdReady => this.CanUseSdk;

        public bool IsStickyAdVisible => this.isStickyAdVisible;

        public bool CallbackLogging
        {
            get => YandexGamesSdk.CallbackLogging;
            set => YandexGamesSdk.CallbackLogging = value;
        }

        public event Action AuthorizedInBackground;

        private bool IsWebGLRuntime => Application.platform == RuntimePlatform.WebGLPlayer && !Application.isEditor;

        private bool CanUseSdk => this.isInitialized && this.IsRunningOnYandex;

        public async UniTask<bool> InitializeAsync(CancellationToken cancellation = default)
        {
            if (!this.IsRunningOnYandex)
                return false;

            if (this.isInitialized)
                return true;

            await YandexGamesSdk.Initialize().ToUniTask(cancellationToken: cancellation);
            this.isInitialized = YandexGamesSdk.IsInitialized;
            return this.isInitialized;
        }

        public void GameReady()
        {
            if (!this.CanUseSdk)
                return;

            YandexGamesSdk.GameReady();
        }

        public YandexGamesEnvironment GetEnvironment()
        {
            if (!this.CanUseSdk)
                return null;

            return YandexGamesSdk.Environment;
        }

        public void StartAuthorizationPolling(int repeatDelay, Action onSuccessCallback = null, Action onErrorCallback = null)
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            PlayerAccount.StartAuthorizationPolling(repeatDelay, onSuccessCallback, onErrorCallback);
        }

        public void Authorize(Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            PlayerAccount.Authorize(onSuccessCallback, onErrorCallback);
        }

        public void RequestPersonalProfileDataPermission(Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            PlayerAccount.RequestPersonalProfileDataPermission(onSuccessCallback, onErrorCallback);
        }

        public void GetProfileData(
            Action<PlayerAccountProfileDataResponse> onSuccessCallback,
            Action<string> onErrorCallback = null,
            ProfilePictureSize pictureSize = ProfilePictureSize.medium
        )
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            PlayerAccount.GetProfileData(onSuccessCallback, onErrorCallback, pictureSize);
        }

        public void SetCloudSaveData(string cloudSaveDataJson, Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            PlayerAccount.SetCloudSaveData(cloudSaveDataJson, onSuccessCallback, onErrorCallback);
        }

        public void GetCloudSaveData(Action<string> onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            PlayerAccount.GetCloudSaveData(onSuccessCallback, onErrorCallback);
        }

        public void SetLeaderboardScore(string leaderboardName, int score, Action onSuccessCallback = null, Action<string> onErrorCallback = null, string extraData = "")
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            Leaderboard.SetScore(leaderboardName, score, onSuccessCallback, onErrorCallback, extraData);
        }

        public void GetLeaderboardEntries(
            string leaderboardName,
            Action<LeaderboardGetEntriesResponse> onSuccessCallback,
            Action<string> onErrorCallback = null,
            int topPlayersCount = 5,
            int competingPlayersCount = 5,
            bool includeSelf = true,
            ProfilePictureSize pictureSize = ProfilePictureSize.medium
        )
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            Leaderboard.GetEntries(leaderboardName, onSuccessCallback, onErrorCallback, topPlayersCount, competingPlayersCount, includeSelf, pictureSize);
        }

        public void GetLeaderboardPlayerEntry(
            string leaderboardName,
            Action<LeaderboardEntryResponse> onSuccessCallback,
            Action<string> onErrorCallback = null,
            ProfilePictureSize pictureSize = ProfilePictureSize.medium
        )
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            Leaderboard.GetPlayerEntry(leaderboardName, onSuccessCallback, onErrorCallback, pictureSize);
        }

        public void PurchaseProduct(string productId, Action<PurchaseProductResponse> onSuccessCallback = null, Action<string> onErrorCallback = null, string developerPayload = "")
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            Billing.PurchaseProduct(productId, onSuccessCallback, onErrorCallback, developerPayload);
        }

        public void ConsumeProduct(string purchasedProductToken, Action onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            Billing.ConsumeProduct(purchasedProductToken, onSuccessCallback, onErrorCallback);
        }

        public void GetProductCatalog(Action<GetProductCatalogResponse> onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            Billing.GetProductCatalog(onSuccessCallback, onErrorCallback);
        }

        public void GetPurchasedProducts(Action<GetPurchasedProductsResponse> onSuccessCallback = null, Action<string> onErrorCallback = null)
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            Billing.GetPurchasedProducts(onSuccessCallback, onErrorCallback);
        }

        public void CanSuggestShortcut(Action<bool> onResultCallback)
        {
            if (!this.CanUseSdk)
            {
                onResultCallback?.Invoke(false);
                return;
            }

            Shortcut.CanSuggest(onResultCallback);
        }

        public void SuggestShortcut(Action<bool> onResultCallback = null)
        {
            if (!this.CanUseSdk)
            {
                onResultCallback?.Invoke(false);
                return;
            }

            Shortcut.Suggest(onResultCallback);
        }

        public void CanOpenReviewPopup(Action<bool, string> onResultCallback)
        {
            if (!this.CanUseSdk)
            {
                onResultCallback?.Invoke(false, SDK_UNAVAILABLE_MESSAGE);
                return;
            }

            ReviewPopup.CanOpen(onResultCallback);
        }

        public void OpenReviewPopup(Action<bool> onResultCallback = null)
        {
            if (!this.CanUseSdk)
            {
                onResultCallback?.Invoke(false);
                return;
            }

            ReviewPopup.Open(onResultCallback);
        }

        public void ShowInterstitialAd(
            Action onOpenCallback = null,
            Action<bool> onCloseCallback = null,
            Action<string> onErrorCallback = null,
            Action onOfflineCallback = null
        )
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            InterstitialAd.Show(onOpenCallback, onCloseCallback, onErrorCallback, onOfflineCallback);
        }

        public void ShowRewardedAd(
            Action onOpenCallback = null,
            Action onRewardedCallback = null,
            Action onCloseCallback = null,
            Action<string> onErrorCallback = null
        )
        {
            if (!this.EnsureSdkAvailable(onErrorCallback))
                return;

            VideoAd.Show(onOpenCallback, onRewardedCallback, onCloseCallback, onErrorCallback);
        }

        public void ShowStickyAd()
        {
            if (!this.CanUseSdk)
                return;

            StickyAd.Show();
            this.isStickyAdVisible = true;
        }

        public void HideStickyAd()
        {
            if (!this.CanUseSdk)
            {
                this.isStickyAdVisible = false;
                return;
            }

            StickyAd.Hide();
            this.isStickyAdVisible = false;
        }

        private bool EnsureSdkAvailable(Action onErrorCallback)
        {
            if (this.CanUseSdk)
                return true;

            onErrorCallback?.Invoke();
            return false;
        }

        private bool EnsureSdkAvailable(Action<string> onErrorCallback)
        {
            if (this.CanUseSdk)
                return true;

            onErrorCallback?.Invoke(this.IsRunningOnYandex ? SDK_NOT_INITIALIZED_MESSAGE : SDK_UNAVAILABLE_MESSAGE);
            return false;
        }

        private void OnAuthorizedInBackground()
        {
            this.AuthorizedInBackground?.Invoke();
        }
    }
}
#endif
