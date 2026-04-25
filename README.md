# WebGL Third Party Service

Wrapper module for WebGL SDKs, following the same pattern as `ThirdParty`.

---

## Poki SDK

Detailed guide:
- `ServiceImplementation/Poki/README.md`

### Structure

```
ServiceImplementation/Poki/
├── PokiUnitySDK.cs               raw SDK (do not modify)
├── PokiSDKService.cs             main service, inject into game code
├── Setup.cs                      bootstraps SDK automatically via VContainer
├── Ads/
│   ├── PokiInterstitialAdsService.cs   plugs into IAdsService
│   └── PokiRewardedAdsService.cs       plugs into IAdsService
└── DI/
    └── PokiVContainer.cs
Plugins/
└── PokiSDKBridge.jslib           JS bridge (WebGL build only)
```

---

### Enabling Poki

Add `POKI` to **Scripting Define Symbols** in Unity Player Settings → **WebGL platform only**.

> Do not enable on Android/iOS — the `DllImport("__Internal")` calls will crash.

---

### SDK Initialization

`Setup.cs` implements `IInitializable` (VContainer) and runs automatically on app start — no manual calls needed.

Automatic sequence:
1. `gameLoadingStart()` — notifies Poki the game is loading
2. `PokiSDK.init()` — initializes the SDK
3. *(SDK ready)* → `Initialize()` ad services → `gameLoadingFinished()`

---

### Injecting and Using `IPokiService`

```csharp
public class GameplayController
{
    private readonly IPokiService pokiService;

    public GameplayController(IPokiService pokiService)
    {
        this.pokiService = pokiService;
    }

    // Call when a level starts
    public void OnLevelStart()
    {
        this.pokiService.GameplayStart();
    }

    // Call when level ends, menu opens, or game pauses
    public void OnLevelEnd()
    {
        this.pokiService.GameplayStop();
    }
}
```

> **Important:** Poki tracks gameplay time to insert ads at the right moment.  
> `GameplayStart` / `GameplayStop` must be called accurately — incorrect usage will impact revenue.

---

### Ads

Ads are routed automatically through `IAdsService` (priority = 10, higher than Dummy = 1).

#### Interstitial

```csharp
this.adsService.ShowInterstitialAd(
    where: "level_complete",
    onShowSuccess: () => Debug.Log("Ad shown"),
    onShowFail: () => Debug.Log("Ad skipped")
);
```

#### Rewarded

```csharp
this.adsService.ShowRewardedAd(
    onComplete: (rewarded) =>
    {
        if (rewarded) GiveReward();
    },
    where: "extra_life"
);
```

> Poki does not support Banner / MREC / AOA / Native — these fall back to `DummyAds`.

---

### API Reference — `IPokiService`

| Method | Description | When to call |
|--------|-------------|--------------|
| `GameplayStart()` | Starts a gameplay session | When the player begins playing |
| `GameplayStop()` | Ends a gameplay session | Menu, pause, game over, level end |
| `GameLoadingStart()` | Signals the game is loading | Handled automatically by `Setup` |
| `GameLoadingFinished()` | Signals the game has loaded | Handled automatically by `Setup` |
| `GetLanguage()` | Returns the user's browser language | Localization |
| `IsAdBlocked()` | Checks if the user has an ad blocker | Hide rewarded button if blocked |
| `LogError(string)` | Sends an error to the Poki dashboard | Catch critical exceptions |

---

## Yandex Games SDK

Detailed guide:
- `ServiceImplementation/Yandex/README.md`

### Structure

```
ServiceImplementation/Yandex/
├── YandexGamesSDKService.cs      main service, wraps BananaParty.YandexGames package
├── YandexCloudSaveService.cs     JSON cloud save wrapper with local cache
├── Setup.cs                      bootstraps the SDK automatically via VContainer
├── Ads/
│   ├── YandexInterstitialAdsService.cs
│   └── YandexRewardedAdsService.cs
├── LocalData/
│   ├── YandexLocalData.cs
│   ├── YandexLocalDataService.cs
│   └── YandexCloudStateLocalDataService.cs
└── DI/
    └── YandexGamesVContainer.cs
Core/
└── Yandex/
    ├── IYandexGamesService.cs
    ├── IYandexCloudSaveService.cs
    ├── IYandexLocalDataService.cs
    └── YandexCloudSaveResult.cs
```

---

### Enabling Yandex

Add `YANDEX_GAMES` to **Scripting Define Symbols** in Unity Player Settings → **WebGL platform only**.

This wrapper expects package `com.bananaparty.yandexgames` to be installed in the project.

Because this module does not modify `Assets/ThirdParty`, register it from your own WebGL-specific `LifetimeScope`:

```csharp
builder.RegisterYandexGamesServices();
```

> Do not enable on Android/iOS/Standalone builds. The underlying SDK only works in WebGL runtime on Yandex Games.

---

### SDK Initialization

`Setup.cs` implements `IAsyncStartable` (VContainer) and runs automatically on app start.

Automatic sequence:
1. Check whether the build is actually running on Yandex Games
2. `YandexGamesSdk.Initialize()`
3. Initialize interstitial/rewarded adapters
4. `YandexGamesSdk.GameReady()`

If the build is opened outside Yandex Games, initialization exits quietly and all wrappers become safe no-ops/fail-fast callbacks.

---

### Injecting and Using `IYandexGamesService`

```csharp
public class YandexBootstrap
{
    private readonly IYandexGamesService yandexGamesService;

    public YandexBootstrap(IYandexGamesService yandexGamesService)
    {
        this.yandexGamesService = yandexGamesService;
    }

    public void ShowRewardedAd()
    {
        this.yandexGamesService.ShowRewardedAd(
            onRewardedCallback: () => UnityEngine.Debug.Log("Reward granted"),
            onCloseCallback: () => UnityEngine.Debug.Log("Ad closed")
        );
    }

    public void LoadCloudSave()
    {
        this.yandexGamesService.GetCloudSaveData(data => UnityEngine.Debug.Log(data));
    }
}
```

---

### Supported Features

`IYandexGamesService` wraps the main features provided by `com.bananaparty.yandexgames`:

- SDK initialization and `GameReady`
- Environment access
- Authorization and profile permission flow
- Profile data and cloud save
- Leaderboards
- Billing
- Shortcut prompt
- Review popup
- Interstitial, rewarded, and sticky ads

Ads are also exposed through `IInterstitialAdsService` / `IRewardedAdsService` adapters inside `ServiceImplementation/Yandex/Ads/`.

---

### Adding a New WebGL SDK

Create a folder `ServiceImplementation/<SDKName>/` with the following structure:

```
ServiceImplementation/
└── CrazyGames/               example
    ├── CrazyGamesSDK.cs
    ├── CrazyGamesService.cs
    ├── Setup.cs
    ├── Ads/
    └── DI/
        └── CrazyGamesVContainer.cs
```

Add the interface under `Core/<SDKName>/`, then register it in `AdsVContainer.cs` behind a `#if <SYMBOL>` guard.
