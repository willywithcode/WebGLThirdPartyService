#if YANDEX_GAMES
namespace WebGLThirdPartyService.Core.Yandex
{
    using System;
    using System.Threading;
    using BananaParty.YandexGames;
    using Cysharp.Threading.Tasks;

    public interface IYandexGamesService
    {
        bool IsRunningOnYandex { get; }
        bool IsInitialized { get; }
        bool IsAuthorized { get; }
        bool HasPersonalProfileDataPermission { get; }
        bool IsInterstitialAdReady { get; }
        bool IsRewardedAdReady { get; }
        bool IsStickyAdVisible { get; }
        bool CallbackLogging { get; set; }

        event Action AuthorizedInBackground;

        UniTask<bool> InitializeAsync(CancellationToken cancellation = default);
        void GameReady();
        YandexGamesEnvironment GetEnvironment();

        void StartAuthorizationPolling(int repeatDelay, Action onSuccessCallback = null, Action onErrorCallback = null);
        void Authorize(Action onSuccessCallback = null, Action<string> onErrorCallback = null);
        void RequestPersonalProfileDataPermission(Action onSuccessCallback = null, Action<string> onErrorCallback = null);
        void GetProfileData(
            Action<PlayerAccountProfileDataResponse> onSuccessCallback,
            Action<string> onErrorCallback = null,
            ProfilePictureSize pictureSize = ProfilePictureSize.medium
        );
        void SetCloudSaveData(string cloudSaveDataJson, Action onSuccessCallback = null, Action<string> onErrorCallback = null);
        void GetCloudSaveData(Action<string> onSuccessCallback = null, Action<string> onErrorCallback = null);

        void SetLeaderboardScore(string leaderboardName, int score, Action onSuccessCallback = null, Action<string> onErrorCallback = null, string extraData = "");
        void GetLeaderboardEntries(
            string leaderboardName,
            Action<LeaderboardGetEntriesResponse> onSuccessCallback,
            Action<string> onErrorCallback = null,
            int topPlayersCount = 5,
            int competingPlayersCount = 5,
            bool includeSelf = true,
            ProfilePictureSize pictureSize = ProfilePictureSize.medium
        );
        void GetLeaderboardPlayerEntry(
            string leaderboardName,
            Action<LeaderboardEntryResponse> onSuccessCallback,
            Action<string> onErrorCallback = null,
            ProfilePictureSize pictureSize = ProfilePictureSize.medium
        );

        void PurchaseProduct(string productId, Action<PurchaseProductResponse> onSuccessCallback = null, Action<string> onErrorCallback = null, string developerPayload = "");
        void ConsumeProduct(string purchasedProductToken, Action onSuccessCallback = null, Action<string> onErrorCallback = null);
        void GetProductCatalog(Action<GetProductCatalogResponse> onSuccessCallback = null, Action<string> onErrorCallback = null);
        void GetPurchasedProducts(Action<GetPurchasedProductsResponse> onSuccessCallback = null, Action<string> onErrorCallback = null);

        void CanSuggestShortcut(Action<bool> onResultCallback);
        void SuggestShortcut(Action<bool> onResultCallback = null);
        void CanOpenReviewPopup(Action<bool, string> onResultCallback);
        void OpenReviewPopup(Action<bool> onResultCallback = null);

        void ShowInterstitialAd(
            Action onOpenCallback = null,
            Action<bool> onCloseCallback = null,
            Action<string> onErrorCallback = null,
            Action onOfflineCallback = null
        );
        void ShowRewardedAd(
            Action onOpenCallback = null,
            Action onRewardedCallback = null,
            Action onCloseCallback = null,
            Action<string> onErrorCallback = null
        );
        void ShowStickyAd();
        void HideStickyAd();
    }
}
#endif
