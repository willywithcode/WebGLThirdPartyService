# Poki SDK

This module wraps the Poki WebGL SDK for use inside `WebGLThirdPartyService`.

## Files

```text
ServiceImplementation/Poki/
├── PokiUnitySDK.cs
├── PokiSDKService.cs
├── Setup.cs
├── Ads/
│   ├── PokiInterstitialAdsService.cs
│   └── PokiRewardedAdsService.cs
└── DI/
    └── PokiVContainer.cs
Plugins/
└── PokiSDKBridge.jslib
```

## Enable

Add `POKI` to `Scripting Define Symbols` for the `WebGL` platform only.

Do not enable it on Android, iOS, or standalone builds.

## Register

Register the services from your WebGL-specific `LifetimeScope`:

```csharp
builder.RegisterPokiServices();
```

## Initialization Flow

`Setup.cs` uses `IInitializable` and starts automatically after DI registration.

Flow:
1. `gameLoadingStart()`
2. `PokiUnitySDK.init()`
3. Wait for SDK ready callback
4. Initialize interstitial and rewarded services
5. `gameLoadingFinished()`

## Main Service

Inject `IPokiService` when you need Poki-specific features:

```csharp
public class GameplayController
{
    private readonly IPokiService pokiService;

    public GameplayController(IPokiService pokiService)
    {
        this.pokiService = pokiService;
    }

    public void OnGameplayStart()
    {
        this.pokiService.GameplayStart();
    }

    public void OnGameplayStop()
    {
        this.pokiService.GameplayStop();
    }
}
```

## Supported Features

- `GameplayStart()`
- `GameplayStop()`
- `GameLoadingStart()`
- `GameLoadingFinished()`
- `GetLanguage()`
- `IsAdBlocked()`
- `LogError(string)`

## Ads

Poki ads are exposed through:
- `PokiInterstitialAdsService`
- `PokiRewardedAdsService`

They implement the shared `ThirdParty` ads interfaces, so they can be selected by the existing ads flow when registered.

### Interstitial

```csharp
this.adsService.ShowInterstitialAd(
    where: "level_complete",
    onShowSuccess: () => UnityEngine.Debug.Log("Interstitial shown"),
    onShowFail: () => UnityEngine.Debug.Log("Interstitial skipped")
);
```

### Rewarded

```csharp
this.adsService.ShowRewardedAd(
    onComplete: rewarded =>
    {
        if (rewarded) GiveReward();
    },
    where: "extra_life"
);
```

## Notes

- Poki uses gameplay start/stop signals for ad timing and revenue optimization.
- Call `GameplayStart()` and `GameplayStop()` accurately around real gameplay.
- Banner, MREC, AOA, and native formats are not implemented here.
