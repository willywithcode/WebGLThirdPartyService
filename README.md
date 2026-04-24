# WebGL Third Party Service

Wrapper module for WebGL SDKs, following the same pattern as `ThirdParty`.

---

## Poki SDK

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
