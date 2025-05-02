# Bolt Unity SDK

<div align="center" style="display:flex;justify-content:center;margin-bottom:20px">
  <img src="https://res.cloudinary.com/dugcmkito/image/upload/v1744983998/bolt_accounts_2x_6c96bccd82.png" alt="Bolt Charge Hero" width="40%" style="padding:20px">

  <img src="https://cdn.sanity.io/images/fuvbjjlp/production/bd6440647fa19b1863cd025fa45f8dad98d33181-2000x2000.png" width="40%" />
</div>

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


## 💰 Why Bolt

Only with Bolt you get **2.1% + $0.30 on transactions**. More than 10x cheaper than traditional app stores which take 30% of your revenue!


<p style="font-size:12px;font-style:italic;opacity:85%">
<strong>Disclaimer:</strong> Fees are subject to change but will continue to remain highly competitive. See <a href="https://www.bolt.com/pricing">bolt.com/pricing</a> for up to date rates.
</p>

## 🚀 Features

This package is a client side Unity SDK
- Bring your own storefront UI
- Open webstore links directly inside your app
- Radically cheaper payment processing rates
- **Future:** User session management

**Have a feature request?** We are constantly improving our SDK support and looking for suggestions. [Join our discord](https://discord.gg/BSUp9qjtnc) and chat with our development team to help get it prioritized!

## 📦 Installation

Add this to your Unity project's `manifest.json`:

```json
"com.yourorg.bolt-unity-sdk": "https://github.com/boltapp/bolt-unity-sdk.git"

```

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
