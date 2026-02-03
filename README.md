# Bolt Unity SDK

<img src="https://github.com/BoltApp/bolt-gameserver-sample/blob/main/public/banner-unity.png?raw=true" />


## What is this?

This SDK provides native Unity support for Bolt Ads and Web Payments.
We also support other platforms:

<table>
  <tr>
    <td align="center" width="150">
      <img src="https://upload.wikimedia.org/wikipedia/commons/6/6a/JavaScript-logo.png" width="60" height="60" alt="JavaScript"/><br>
      <b>JavaScript</b><br>
      <a href="https://github.com/BoltApp/bolt-frontend-sdk">Javascript SDK</a>
    </td>
    <td align="center" width="150" bgcolor="#b688ff">
      <img src="https://cdn.sanity.io/images/fuvbjjlp/production/bd6440647fa19b1863cd025fa45f8dad98d33181-2000x2000.png" width="60" height="60" alt="Unity"/><br>
      <div style="color: black">
      <b>Unity</b><br>
      <i>This Repo</i>
      </div>
    </td>
    <td align="center" width="150">
      <img src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRUf3R8LFTgqC_8mooGEx7Fpas9kHu8OUxhLA&s" width="60" height="60" alt="Unreal"/><br>
      <b>Unreal Engine</b><br>
      <a href="https://github.com/BoltApp/bolt-unreal-engine-sdk">Unreal SDK</a>
    </td>
  </tr>
  <tr>
    <td align="center" width="150">
      <img src="https://developer.apple.com/assets/elements/icons/swift/swift-64x64.png" width="60" height="60" alt="iOS"/><br>
      <b>iOS</b><br>
      Coming Soon 🚧
    </td>
    <td align="center" width="150">
      <img src="https://avatars.githubusercontent.com/u/32689599?s=200&v=4" width="60" height="60" alt="Android"/><br>
      <b>Android</b><br>
      Coming Soon 🚧
    </td>
    <td align="center" width="150">
      <!-- filler -->
    </td>
  </tr>
</table>

<br>

<div align="center">

### Chat with us on Discord for help and inquiries!

[![Discord](https://img.shields.io/badge/Discord-Have%20A%20Request%3F-7289DA?style=for-the-badge&logo=discord&logoColor=white&logoWidth=60)](https://discord.gg/BSUp9qjtnc)

</div>

## 📚 Documentation

For further documentation and API reference visit our [Quickstart guide](https://gaming-help.bolt.com/guide/quickstart.html).

## 💰 Why Bolt

Only with Bolt you get **2.1% + $0.30 on all transactions**. That's 10x better than traditional app stores which take 30% of your revenue! That's the fair and transparent pricing you get with using Bolt.

> Bolt’s fees are subject to change but will remain highly competitive. 
> For the latest rates, see [Bolt Pricing](https://www.bolt.com/pricing). 
> For details, review the [End User Terms and Conditions](https://www.bolt.com/end-user-terms).

## 🛠️ Prerequisites

You need 3 things to get started:

1. **Existing App:** You will need an application in the same platform as this SDK
2. **Backend Server:** You will need to bring your own backend server (any language)
3. **Bolt Merchant Account:** Dashboard access to manage your store ([sign up](https://merchant.bolt.com/onboarding/get-started/gaming) or [log in](https://merchant.bolt.com/))
4. **UniWebViewAdService.cs** [Unity plugin supporting iOS and Android in-game webviews](https://docs.uniwebview.com/guide/)



## 📦 Installation

### Step 0: Install UniWebView
UniWebView is a Unity plugin supporting in-game webviews for iOS and Android mobile games. In order to install, [please refer to the UniWebView installation guide here](https://docs.uniwebview.com/guide/). 

**Note:** You are open to follow the installation method of your preference. Most convenient options would be through the Unity Asset Store or UniWebView's own web store. 

### Step 1: Install the Unity SDK

**Note:** For any of these options you can specify a specific version by appending it to the URL with a hashtag, e.g. `https://github.com/BoltApp/bolt-unity-sdk.git#v0.0.5` will pin `v0.0.5`

#### Option 1: Using manifest.json (Recommended)

1. Open your Unity project
2. Navigate to the Packages folder in your project root
3. Open the `manifest.json` file in a text editor
4. Add the Bolt SDK dependency to the `dependencies` section:

```json
{
  "dependencies": {
    "com.bolt.sdk": "https://github.com/BoltApp/bolt-unity-sdk.git#main",
    // ... other dependencies
  }
}
```

5. **Save the file** - Unity will automatically download and import the package

#### Option 2: Using Package Manager UI

1. Open Package Manager in Unity (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL"
4. Enter the repository URL: `https://github.com/BoltApp/bolt-unity-sdk.git#main`
5. Click "Add"

If you have any issues our discord support team will be happy to help.

### Step 3: Update Unity Project Settings to Include New Plugins
Under your game's project settings, go to `Player` and make the following changes:
1. Find the `Other Settings` section, at the bottom of this section you will see `Script Compilation`
2. Add both `BOLT_SDK` and `UNIWEBVIEW` as scripting define symbols

### Step 4: Add code to your game

There are various sample integrations in the `Samples~/` folder. 
#### Ads Integration
1. Copy these files from `Samples~/AdsIntegration/` to your game's source code:
- [**BoltClient.cs**](./Samples~/AdsIntegration/BoltClient.cs): Your game's scripts folder (e.g., `Assets/Scripts/Bolt/`)
- [**UniWebViewAdService.cs**](./Samples~/AdsIntegration/UniWebViewAdService.cs): Your game's service layer (e.g., `Assets/Scripts/Services/`)

2. Update both `BoltClient.cs` and `UniWebViewAdService.cs` so their namespaces reference your project's paths

3. Update `BoltClient.cs` in the `InitializeBoltSDK()` method (around lines 13-14)
- `gameId` - Your Bolt game ID (replace `"com.myapp.test"`)
- `publishableKey` - Your Bolt publishable key (replace `"example.publishable.key"`)
- `Environment` - Set to `BoltConfig.Environment.Development`, `BoltConfig.Environment.Sandbox`, or `BoltConfig.Environment.Production` (currently defaults to `Sandbox` on line 36)

### Step 4: Add the BoltClient to Your Scene
1. Create an empty GameObject in your scene
2. Add the `BoltClient` component to it
3. The client will initialize automatically on `Start()`

### Step 5: Begin Displaying Ads with Button Attachment
 Start by setting up a button in Unity:
 1. In your code, create a new function that calls the BoltClient's `ShowAd()` function
 2. In Unity, click on the asset you want to trigger the ad and attach the function via the Unity Inspector's On Click() button setup

### Example Function
The client already handles initialization, ad preloading, etc. so at this point you can simply add the following function call 
```csharp
    public void ExampleAdButton()
    {
      // ... add any other button logic required
        BoltClient.Instance.ShowAd();
    }
```

### Step 6: Handle Ad Lifecycle Events
The `OnAdOpened()` and `OnAdCompleted()` methods in `BoltClient` allow you to add things like UI updates, game state management, player rewards, etc. 

The ad page uses UniWebView's Channel Messaging API to send a callback when the user closes a given ad. 
- When the claim button is clicked, the ad page calls `window.uniwebview.send('bolt-gaming-issue-reward', ...)`
- `UniWebViewAdService` catches this via `OnChannelMessageReceived`
- This fires the `onAdCompleted` event in your `BoltClient`

**What this means for you:**
- The callback is handled automatically
- To handle rewards logic, update BoltClient's `OnAdCompleted()` (line 95)

## Gotchas and Important Notes
### 1. UniWebViewAdService Must Be in Game Code
 The SDK doesn't include UniWebView as a dependency. Your game provides the implementation via `IAdWebViewService`.

Copy `UniWebViewAdService.cs` anywhere that your BoltClient.cs file can reference, not the SDK folder.

### 2. Idepmotency
While not necessary, it is safe to call the SDK's `preloadAd` function multiple times. The webview is created once and reused. 
Only one ad can be active at a time. If you call `ShowAd` while an ad is already showing, the previous session will be replaced.


## Web Payments
Here are some details on the payments integration which has some key differences to the Ads example above. 
### Integration Examples
Integration examples are also provided in the `Samples~/` folder. 
- [**BoltBasicExample**](./Samples~/BasicIntegration/BoltBasicExample.cs): will showcase how to initialize the client and check for pending transactions
- [**BoltDeepLinkExample**](./Samples~/DeepLinkIntegration/BoltDeepLinkExample.cs): will showcase how to handle deep links back into the application.
### Backend Integration
You will need to bring your own backend server to complete integration for web payments. 
- [**Quickstart**](https://gaming-help.bolt.com/guide/quickstart.html): View our quickstart guide to get the API running
- [**Example Server**](https://github.com/BoltApp/bolt-gameserver-sample): We also have a sample server in NodeJS for your reference during implementation


## Need help?

<div class="discord-link">
    Got questions, roadmap suggestions, or requesting new SDKs?
    <br>
    <a href="https://discord.gg/BSUp9qjtnc" 
    target="_blank" class="discord-link-anchor">
      <span class="discord-text mr-2">Get help and chat with 
      us about anything on Discord</span>
      <span class="discord-icon-wrapper">
        <img src="https://cdn.prod.website-files.com/6257adef93867e50d84d30e2/66e3d80db9971f10a9757c99_Symbol.svg"
        alt="Discord" class="discord-icon" 
        width="16px">
      </span>
    </a>
  </div>



## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
