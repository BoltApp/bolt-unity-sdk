# Bolt SDK Ad Integration Guide

This guide explains how to integrate Bolt SDK ads into your Unity game.

## Overview

The Bolt SDK ad system requires:
1. Installing UniWebView to your source: https://docs.uniwebview.com/guide/installation.html 
2. **BoltClient.cs** - Main client class that manages the SDK
3. **UniWebViewAdService.cs** - WebView service implementation (must be in your game code)
4. **Bolt SDK** - The SDK package itself

## Setup Steps

### 1. Purchase and Install UniWebView 
This library can be purchased via the Unity Asset Store or direct from UniWebView. For more details, see here: https://docs.uniwebview.com/guide/installation.html

### 2. Install the Bolt SDK

### 3. Copy Required Files to Your Game

Copy these files from `Samples~/AdIntegration/` to your game's source code:

- **BoltClient.cs** → Your game's scripts folder (e.g., `Assets/Scripts/Bolt/`)
- **UniWebViewAdService.cs** → Your game's service layer (e.g., `Assets/Scripts/Services/`)

**Important:** `UniWebViewAdService.cs` must live in your game code, not in the SDK package, because it depends on UniWebView, which will be added to your own game's available assets.

### 4. Configure BoltClient

Update `BoltClient.cs` in the `InitializeBoltSDK()` method (around line 28-31):
- `gameId` - Your Bolt game ID (replace `"com.myapp.test"`)
- `publishableKey` - Your Bolt publishable key (replace `"example.publishable.key"`)
- `Environment` - Set to `BoltConfig.Environment.Development`, `BoltConfig.Environment.Staging`, or `BoltConfig.Environment.Production` (currently defaults to `Development` on line 31)

### 4. Add BoltClient to Scene

1. Create an empty GameObject in your scene
2. Add the `BoltClient` component to it
3. The client will initialize automatically on `Start()`

## Usage Examples

### Basic Usage (Recommended)

The ad is preloaded once during initialization, then you can show it many times:

```csharp
var boltClient = FindObjectOfType<BoltClient>();

// Get ad link from your backend
var adLink = "https://your-backend.com/get-ad-link";

// Preload ad (call once at game start)
boltClient.PreloadAd(adLink);

// Show ad from button click, etc. (can call many times)
await boltClient.ShowAd();
```

## Gotchas and Important Notes

### 1. UniWebViewAdService Must Be in Game Code
 The SDK doesn't include UniWebView as a dependency. Your game provides the implementation via `IAdWebViewService`.

Copy `UniWebViewAdService.cs` anywhere that your BoltClient.cs file can reference, not the SDK folder.

### 3. onClaim Callback

The ad completion is detected via the `onClaim` callback from the ad URL. This is handled automatically by:
- `UniWebViewAdService` - Listens for the callback
- `BoltSDK` - Fires `onAdCompleted` event when received
- `BoltClient` - exposes handling of the event in `OnAdCompleted()`

**No manual intervention needed** - just implement your reward logic in `OnAdCompleted()`.

### 4. Single Active Session

Only one ad can be active at a time. If you call `ShowTimedAd()` or `ShowUntimedAd()` while an ad is already showing, the previous session will be replaced.

### 5. PreloadAd is Idempotent

You can call `PreloadAd()` multiple times safely. The webview is created once and reused.

### 6. Event Handlers

The `OnAdOpened()` and `OnAdCompleted()` methods in `BoltClient` allot you to add things like UI updates, game state management, player rewards