# Bolt Unity SDK

<div align="center" style="display:flex;justify-content:center;margin-bottom:20px">
  <img src="https://res.cloudinary.com/dugcmkito/image/upload/v1744983998/bolt_accounts_2x_6c96bccd82.png" alt="Bolt Charge Hero" height="250px">
  <img src="https://cdn.sanity.io/images/fuvbjjlp/production/bd6440647fa19b1863cd025fa45f8dad98d33181-2000x2000.png" height="250px" />
</div>


## What is this?

Native Unity support for [Bolt Charge](https://www.bolt.com/charge), a fully hosted webshop for out-of-app purchases and subscriptions.

We also support other platforms:

<table>
  <tr>
    <td align="center" width="150">
      <img src="https://upload.wikimedia.org/wikipedia/commons/6/6a/JavaScript-logo.png" width="60" height="60" alt="JavaScript"/><br>
      <b>JavaScript</b><br>
      <a href="https://github.com/BoltApp/bolt-frontend-sdk">Frontend Web SDK</a>
    </td>
    <td align="center" width="150" bgcolor="#b688ff">
      <img src="https://cdn.sanity.io/images/fuvbjjlp/production/bd6440647fa19b1863cd025fa45f8dad98d33181-2000x2000.png" width="60" height="60" alt="Unity"/><br>
      <div style="color: black">
      <b>Unity</b><br>
      <i>This Repo</i>
      </div>
    </td>
    <td align="center" width="150">
      <img src="https://camo.githubusercontent.com/416d2f81dcfee6991a059240e349bb4dc47896c63f7a775cd4fae482cec2e7c2/68747470733a2f2f63646e322e756e7265616c656e67696e652e636f6d2f75652d6c6f676f747970652d323032332d766572746963616c2d77686974652d3136383678323034382d6262666465643236646161372e706e67" width="40" height="40" style="background: black; padding: 10px" alt="Unreal"/><br>
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

<div style="border: 1px solid #b688ff; background-color: rgba(182, 136, 255, 0.1); padding: 15px; border-radius: 8px;margin-bottom: 16px;">
<b>Note:</b> This Unity SDK is still in early access and requires a manual install. Official package support is planned for the near future.
</div>

This project depends on [unity-webview](https://github.com/gree/unity-webview) plugin.

For both this Bolt Unity SDK and the [unity-webview](https://github.com/gree/unity-webview) plugin:
1. Download the repos as a zip file:
    - [Bolt Unity Zip](https://github.com/BoltApp/bolt-unity-sdk/archive/refs/heads/main.zip)
    - [Unity Webview Zip](https://github.com/gree/unity-webview/archive/refs/heads/master.zip)
2. Unpack it and drag it into your project's `Assets/` folder
3. The next section will help you resolve errors

#### ⚠️ Fix unity-webview issues
The `unity-webview` package is finicky to install because it has an example project inside of it. Please follow the next steps carefully.

Once you have the unzipped folder in your assets folder, make sure to run the `dist/unity-webview.unitypackage` file which will import the necessary files into your project. 

You can then delete the unzipped unity-webview folder you just added to `/Assets`. This should also resolve import errors from the Bolt sdk package.

Review the [General Notes](https://github.com/gree/unity-webview?tab=readme-ov-file#general-notes) to ensure you resolve any package errors.

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
    public string serverUrl = "https://your-server.herokuapp.com";
    
    private BoltApiService boltApi;
    private WebViewManager webViewManager;

    void Start()
    {
        // Set up the payment system
        boltApi = new BoltApiService(serverUrl);
        webViewManager = gameObject.AddComponent<WebViewManager>();
        webViewManager.OnWebViewClosed += OnPaymentComplete;
    }

    // Call this when player wants to buy something
    // Ensure to style the modal to your preference and include a close button
    public void BuyItem(string paymentUrl)
    {
        webViewManager.OpenFullScreenWebView(paymentUrl);
    }

    // This runs when payment is done
    void OnPaymentComplete()
    {
        Debug.Log("Payment finished!");

        // Recommended: you can sync your player object by polling your backend since a transaction webhook will have hit your backend server.

        // Optional: If you have a simple checkout flow, you can use Bolt's api to check if player bought the item
        StartCoroutine(boltApi.GetUserSubscriptions("player@email.com"));
    }
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
