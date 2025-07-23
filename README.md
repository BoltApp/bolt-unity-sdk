# Bolt Unity SDK

<img src="https://github.com/BoltApp/bolt-gameserver-sample/blob/main/public/banner-unity.png?raw=true" />


## What is this?

Native Unity support for [Bolt Charge](https://www.bolt.com/charge), a fully hosted webshop for out-of-app purchases and subscriptions.

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

## 📚 Documentation

For broad documentation and API reference visit our [documentation site](https://docs.bolt.com).


## 📦 Installation


### Step 1: Install the Unity SDK

#### Option 1: Using manifest.json (Recommended)

1. **Open your Unity project**
2. **Navigate to the Packages folder** in your project root
3. **Open the `manifest.json` file** in a text editor
4. **Add the Bolt SDK dependency** to the `dependencies` section:

```json
{
  "dependencies": {
    "com.bolt.sdk": "https://github.com/BoltApp/bolt-unity-sdk.git#v0.0.1",
    // ... other dependencies
  }
}
```

5. **Save the file** - Unity will automatically download and import the package

#### Option 2: Using Package Manager UI

1. **Open Package Manager** in Unity (Window > Package Manager)
2. **Click the "+" button** in the top-left corner
3. **Select "Add package from git URL"**
4. **Enter the repository URL**: `https://github.com/BoltApp/bolt-unity-sdk.git`
5. **Click "Add"**

#### Option 3: Using Git URL

1. **Open Package Manager** in Unity (Window > Package Manager)
2. **Click the "+" button** in the top-left corner
3. **Select "Add package from git URL"**
4. **Enter the repository URL**: `https://github.com/BoltApp/bolt-unity-sdk.git`
5. **Click "Add"**

**Note:** You can also specify a specific version by adding `#v0.0.1` to the URL.

For detailed installation instructions and troubleshooting, see [INSTALLATION.md](INSTALLATION.md).

If you have any issues our discord support team will be happy to help.

### Step 2: Set up your backend server

You need to bring your own server to safely handle transactions and api keys.
1. Integrate the Bolt API
    - This is how you will interact with the Charge API and manage digital subscriptions
    - Docs: https://help.bolt.com/products/bolt-charge/charge-setup/ 
    - API: https://help.bolt.com/api-subscriptions/ 
    - Example server: https://github.com/BoltApp/bolt-gameserver-sample
2. Set up the Authorization Webhook
    - _"Authorization"_ is an industry term for transactions
    - This is how you will check if a user completed a transaction
    - Webhook Docs: https://help.bolt.com/developers/webhooks/webhooks
    - Webhook Events: https://help.bolt.com/developers/webhooks/webhooks/#authorization-events
    - API: https://help.bolt.com/api-merchant/#tag/webhooks/POST/webhooks_transaction 
3. Note your server URL (like `https://your-server.herokuapp.com`)
    - You will use this URL for initializing the api client in Step 4
    - Consider using configs for managing different environments

### Step 3: Get your Bolt account

1. Go to [merchant.bolt.com](https://www.merchant.bolt.com) and login to the dashboard. You can [signup here](https://merchant.bolt.com/onboarding/get-started) if you don't have an account.
2. Set up your products in the Bolt dashboard. You can find [helpful instructions in our documentation](https://help.bolt.com/products/bolt-charge/charge-setup/#set-up-your-products).
3. Get your checkout links (they look like: `https://digital-subscriptions-test-14-04.c-staging.bolt.com/c?u=SRZKjocdzkUmJfS2J7JNCQ&publishable_key=BQ9PKQksUGtj.Q9LwVLfV3WF4.32122926f7b9651a416a5099dc92dc2b4c87c8b922c114229f83b345d65f4695`)

### Step 4: Add code to your game

You may copy this code into a new script in your Unity project or use it for reference on how to initialize the bolt client and webview.

```c#
using UnityEngine;

public class BoltPayments : MonoBehaviour
{
    [Header("Your Backend Server")]
    // TODO make this configurable from editor
    public string serverUrl = "https://your-server.herokuapp.com";
    
    
    void Start() {
      // TODO
    }

    // Need code for:
    // GET/SET player prefs
    // -   player email
    // -   player locale and country
    // -   name of app (used for deeplinking, maybe a bolt specific one)
    // -   set new transaction to be acknowledged (we open browser so don't know if it worked)
    // -   on app load, see if last transaction was acknowledged
    // Opening website utility code
    // - open URL by string, append query parameters for device locale, user email, device country, app name for deeplinking back
    // - getting products by ID (key value pairs)
    // Deeplinking sample code to read transaction data and acknowledge the last transaction
}
```

## Step 5: Test it

1. Add the script to a GameObject in your scene
2. Put your server URL in the `serverUrl` field
3. Call `BuyItem()` with a Bolt payment link
    - **Note:** You can use our staging url for testing purposes: https://digital-subscriptions-test-14-04.c-staging.bolt.com/c?u=SRZKjocdzkUmJfS2J7JNCQ&publishable_key=BQ9PKQksUGtj.Q9LwVLfV3WF4.32122926f7b9651a416a5099dc92dc2b4c87c8b922c114229f83b345d65f4695
4. The payment page should open as a modal in your game
5. Modify the modal style to your liking. Ensure to add a close button and handle appropriately.

**Congratulations 🎉**
<br>You have successfully integrated Bolt Charge into your app! 

## Next Steps

Now that you have a single checkout working, you will want to adopt some best practices to make them easy to maintain.

#### Configs
Use a config for managing your collection of checkout links. We recommend using JSON and mapping links to readable names. You can swap configs based on environment. Example:
```
{
  GEMS_100:   'https://your-checkout-link-here.com',
  GEMS_500:   'https://your-checkout-link-here.com',
  GEMS_1000:  'https://your-checkout-link-here.com',
  BUNDLE_ONE: 'https://your-checkout-link-here.com',
  BUNDLE_TWO: 'https://your-checkout-link-here.com'
  // etc...
}
```

#### Integration Tests
We recommend setting up automated testing against the most common flows. Good test coverage should include UI or API test coverage of the following scenarios:
- Checkout is possible to open
- Checkout is possible to close
- User gets success state from successful transaction
- User gets failed state from failed transaction
- User network interrupted after good payment, is shown success screen on reboot of app
- User network interrupted after bad payment, is shown fail screen on reboot of app

#### Translations 🚧

Bolt does support translations and handles many checkouts on the global market. However, right now the SDK is tailored to the U.S. market so only English is officially provided.

We will be rolling out official multi-region support to Bolt Charge in the very near future. If you would like a preview or are curious about the timeline, you can reach out to our team directly.

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