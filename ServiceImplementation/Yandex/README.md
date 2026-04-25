# Yandex Games SDK

This module wraps `com.bananaparty.yandexgames` for use inside `WebGLThirdPartyService`.

## Files

```text
Core/Yandex/
└── IYandexGamesService.cs

ServiceImplementation/Yandex/
├── YandexGamesSDKService.cs
├── YandexCloudSaveService.cs
├── Setup.cs
├── Ads/
│   ├── YandexInterstitialAdsService.cs
│   └── YandexRewardedAdsService.cs
├── LocalData/
│   ├── YandexLocalData.cs
│   ├── YandexLocalDataService.cs
│   └── YandexCloudStateLocalDataService.cs
└── DI/
    └── YandexGamesVContainer.cs
```

## Requirements

- Package `com.bananaparty.yandexgames` must be installed
- Add `YANDEX_GAMES` to `Scripting Define Symbols` for the `WebGL` platform only

Do not enable it on Android, iOS, or standalone builds.

## Register

Register the services from your WebGL-specific `LifetimeScope`:

```csharp
builder.RegisterYandexGamesServices();
```

This module does not modify `Assets/ThirdParty`, so registration must happen from your project bootstrap.

## Initialization Flow

`Setup.cs` uses `IAsyncStartable`.

Flow:
1. Check if the build is running on Yandex Games
2. `YandexGamesSdk.Initialize()`
3. Initialize interstitial and rewarded adapters
4. `YandexGamesSdk.GameReady()`

If the build is not running inside Yandex Games, initialization exits quietly.

## Main Service

Inject `IYandexGamesService` when you need Yandex-specific features:

```csharp
public class YandexBootstrap
{
    private readonly IYandexGamesService yandexGamesService;

    public YandexBootstrap(IYandexGamesService yandexGamesService)
    {
        this.yandexGamesService = yandexGamesService;
    }

    public void LoadCloudSave()
    {
        this.yandexGamesService.GetCloudSaveData(data => UnityEngine.Debug.Log(data));
    }

    public void ShowRewarded()
    {
        this.yandexGamesService.ShowRewardedAd(
            onRewardedCallback: () => UnityEngine.Debug.Log("Reward granted"),
            onCloseCallback: () => UnityEngine.Debug.Log("Ad closed")
        );
    }
}
```

## Supported Features

- SDK initialization
- `GameReady()`
- Environment access
- Authorization and authorization polling
- Personal profile data permission flow
- Profile data
- Cloud save
- Leaderboard score / entries / player entry
- Billing purchase / consume / catalog / purchased products
- Shortcut prompt
- Review popup
- Interstitial ads
- Rewarded ads
- Sticky ads
- Cloud save wrapper with local cache and pending-save state

## Ads

Yandex ads are exposed through:
- `YandexInterstitialAdsService`
- `YandexRewardedAdsService`

They implement the shared `ThirdParty` ads interfaces, so they can be wired into the existing ads pipeline once registered by your project.

### Interstitial

```csharp
this.adsService.ShowInterstitialAd(
    where: "level_complete",
    onShowSuccess: () => UnityEngine.Debug.Log("Interstitial shown"),
    onShowFail: () => UnityEngine.Debug.Log("Interstitial failed")
);
```

### Rewarded

```csharp
this.adsService.ShowRewardedAd(
    onComplete: rewarded =>
    {
        if (rewarded) GiveReward();
    },
    where: "continue"
);
```

## Notes

- The wrapper is defensive outside real Yandex runtime and falls back to no-op or fail-fast callbacks.
- `IYandexGamesService` is the correct place for Yandex-only features like profile, cloud save, billing, and review popup.
- This module does not patch `Assets/ThirdParty`; it only provides the provider implementation.

## Cloud Save Service

`IYandexCloudSaveService` is a JSON-based wrapper on top of Yandex cloud save.

It provides:
- `SaveAsync(string json)`
- `LoadAsync(...)`
- local cache stored through `YandexCloudStateLocalDataService`
- pending payload tracking when runtime is unavailable or a save fails

`YandexLocalDataService<T>` is the Yandex-specific base local-data layer:
- `YandexLocalDataService<T> : ILocalDataService<T>`

The current concrete implementation is:
- `YandexLocalData : ILocalData`
- `YandexCloudStateLocalDataService : YandexLocalDataService<YandexLocalData>`

Example:

```csharp
public class YandexSaveExample
{
    private readonly IYandexCloudSaveService yandexCloudSaveService;

    public YandexSaveExample(IYandexCloudSaveService yandexCloudSaveService)
    {
        this.yandexCloudSaveService = yandexCloudSaveService;
    }

    public async Cysharp.Threading.Tasks.UniTask SaveAsync(string json)
    {
        var result = await this.yandexCloudSaveService.SaveAsync(json);
        if (!result.IsSuccess)
        {
            UnityEngine.Debug.LogWarning(result.ErrorMessage);
        }
    }

    public async Cysharp.Threading.Tasks.UniTask<string> LoadAsync()
    {
        var result = await this.yandexCloudSaveService.LoadAsync();
        return result.IsSuccess ? result.CloudSaveJson : "{}";
    }
}
```
