# Bolt Unity SDK

<div align="center" style="display:flex;justify-content:center;margin-bottom:20px">
  <img src="https://res.cloudinary.com/dugcmkito/image/upload/v1744983998/bolt_accounts_2x_6c96bccd82.png" alt="Bolt Charge Hero" width="40%" style="padding:20px">

  <img src="https://cdn.sanity.io/images/fuvbjjlp/production/bd6440647fa19b1863cd025fa45f8dad98d33181-2000x2000.png" width="40%" />
</div>

<br>

Native Unity support for [Bolt Charge](https://www.bolt.com/charge), a fully hosted webshop for out-of-app purchases and subscriptions.

<div class="discord-link">
    Got Questions?
    <a href="https://discord.gg/BSUp9qjtnc" target="_blank" class="discord-link-anchor">
      <span class="discord-text mr-2">Chat with us on Discord</span>
      <span class="discord-icon-wrapper">
        <img src="https://help.bolt.com/images/brand/Discord-Symbol-White.svg" alt="Discord" class="discord-icon" width="15px">
      </span>
    </a>
  </div>

<br>

We also have [Native Unreal Engine](https://github.com/davidminin/bolt-unreal-engine-sdk) Support and a [Sample Backend](https://github.com/davidminin/bolt-gameserver-sample) for additional reference.

## 💰 Why Bolt

Only with Bolt you get **2.1% + $0.30 on all transactions**. That's 10x better than traditional app stores which take 30% of your revenue! That's the fair and transparent pricing you get with using Bolt.

<p style="font-size:12px;font-style:italic;opacity:85%">
<strong>Disclaimer:</strong> Fees are subject to change but will continue to remain highly competitive. See <a href="https://www.bolt.com/pricing">bolt.com/pricing</a> for up to date rates and visit  <a href="https://www.bolt.com/end-user-terms">bolt.com/end-user-terms</a> for end user terms and conditions.
</p>

## 🚀 Features

This open source package is a light-weight Unity SDK.
- Bring your own UI
- Open webstore links directly inside your app
- Radically cheaper payment processing rates
- **Future:** User session management

The SDK supports Unity versions LTS 2021.3 to Unity 6. Code might work for older versions of Unity but is not officially supported.

**Have a feature request?** We are constantly improving our SDKs and looking for suggestions. [Join our discord](https://discord.gg/BSUp9qjtnc) and chat directly with our development team to help with our roadmap!

## 📦 Installation

This project depends on [unity-webview](https://github.com/gree/unity-webview) plugin. Both the webview plugin and the bolt unity SDK require manual installations.

**Note:** This project is still in early access but we plan to provide official package support in the near future.

For both the [unity-webview](https://github.com/gree/unity-webview) plugin and for the bolt-unity-sdk:
1. Download the repo as a zip file
2. Unpack it and drag it into your project's `Assets/` folder
3. Follow readme steps to resolve errors

The `unity-webview` package is particularly complicated to install because it has an example project inside of it. 
> Once you have the unzipped folder in your assets folder, make sure to run the `dist/unity-webview.unitypackage` file which will import the necessary files into your project. 
>
> You can then delete the unzipped unity-webview folder you just added to `/Assets`. This should also resolve import errors from the Bolt sdk package.
>
> If you have any issues our discord support team will be happy to help.


## 🔧 Quick Start

**Requirements:** You must have a backend server for proxying API requests. See our [server sample](https://github.com/davidminin/bolt-gameserver-sample) for an example integration.

1. Add the SDK to your project
2. Add routes to your backend server [(see example usage)](https://github.com/davidminin/bolt-gameserver-sample/blob/main/example-usage.ts)
3. Use the staging api configs to test purchases in your dev environment

<br>

**Example Usage:**
```c#
using UnityEngine;

public class BoltDemo : MonoBehaviour
{
    [Header("Backend Config")]
    public string backendBaseUrl = "https://your-backend-server.com";

    private BoltApiService boltApi;
    private WebViewManager webViewManager;

    void Start()
    {
        // Initialize Bolt SDK with your backend server URL
        boltApi = new BoltApiService(backendBaseUrl);

        // Add WebViewManager component at runtime
        webViewManager = gameObject.AddComponent<WebViewManager>();
        webViewManager.OnWebViewClosed += HandleWebViewClosed;

        // Fetch subscription data for specific user
        StartCoroutine(boltApi.GetUserSubscriptions("test@test.com"));

        // Open full screen webview (e.g. for purchasing a product). 
        // It's recommended to embed URLs as data attributes into your in-game objects or to maintain a list using a helper class.
        // You can generate these links using the bolt dashboard https://help.bolt.com/products/bolt-charge/charge-setup/#set-up-your-products
        OpenCheckoutPage("https://digital-subscriptions-test-14-04.c-staging.bolt.com/c?u=SRZKjocdzkUmJfS2J7JNCQ&publishable_key=BQ9PKQksUGtj.Q9LwVLfV3WF4.32122926f7b9651a416a5099dc92dc2b4c87c8b922c114229f83b345d65f4695");
    }

    public void OpenCheckoutPage(string webUrl)
    {
        webViewManager.OpenFullScreenWebView(webUrl);
    }

    public void CloseCheckoutPage()
    {
        webViewManager.CloseWebView();
    }

    void HandleWebViewClosed()
    {
        Debug.Log("WebView was closed by the user.");
        // Trigger anything you want here (check ownership of subscription, refresh UI, resume game, etc.)
    }
}

```

## 📚 Documentation

For detailed documentation and API reference, visit our [documentation site](https://docs.bolt.com).


## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
