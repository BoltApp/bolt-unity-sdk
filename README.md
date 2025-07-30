# Bolt Unity SDK

<img src="https://github.com/BoltApp/bolt-gameserver-sample/blob/main/public/banner-unity.png?raw=true" />


## What is this?

Native Unity support for Bolt Web Payments. A programmatic way to for out-of-app purchases and subscriptions.

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

[![Discord](https://img.shields.io/badge/Discord-Have%20A%20Request%3F-7289DA?style=for-the-badge&logo=discord&logoColor=white&logoWidth=60)](https://discord.gg/BSUp9qjtnc)

### Chat with us on Discord for help and inquiries!

</div>

## 📚 Documentation

For broad documentation and API reference visit our [documentation site](https://docs.bolt.com).

## 💰 Why Bolt

Only with Bolt you get **2.1% + $0.30 on all transactions**. That's 10x better than traditional app stores which take 30% of your revenue! That's the fair and transparent pricing you get with using Bolt.

<p style="font-size:12px;font-style:italic;opacity:85%">
<strong>Disclaimer:</strong> Fees are subject to change but will continue to remain highly competitive. See <a href="https://www.bolt.com/pricing">bolt.com/pricing</a> for up to date rates and visit  <a href="https://www.bolt.com/end-user-terms">bolt.com/end-user-terms</a> for end user terms and conditions.
</p>

## 🛠️ Prerequisites

You need 3 things to get started:

1. **Existing App:** You will need an application in the same platform as this SDK
2. **Backend Server:** You will need to bring your own backend server (any language)
3. **Bolt Merchant Account:** Dashboard access to manage your store ([signup](https://merchant.bolt.com/onboarding/get-started) or [login](https://merchant.bolt.com/))



## 📦 Installation

### Step 1: Install the Unity SDK

**Note:** For any of these options you can specify a specific version by appending it to the URL with a hashtag, e.g. `https://github.com/BoltApp/bolt-unity-sdk.git#v0.0.2` will pin `v0.0.2`

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

### Step 2: Add code to your game

There is sample integrations in the `Samples~/` folder. 
- [**BoltBasicExample**](./Samples~/BasicIntegration/BoltBasicExample.cs): will showcase how to initialize the client and check for pending transactions
- [**BoltDeepLinkExample**](./Samples~/DeepLinkIntegration/BoltDeepLinkExample.cs): will showcase how to handle deep links back into the application.

### Step 3: Continue with Backend Integration
You will need to implement some features in your backend for secure link and transaction handling. 
- Generate Checkout Links (Beta usage only, DM our team for CURL examples)
- [Handle Transaction Webhooks](https://help.bolt.com/api-merchant/#tag/webhooks)

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