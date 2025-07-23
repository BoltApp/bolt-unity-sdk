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

1. Download the latest release from this repository
2. Import the Unity package into your project
3. The SDK will be available in the `BoltSDK` namespace

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

## 🚀 Quick Start

### Basic Usage

#### Option 1: Using Configuration Asset (Recommended)

1. Create a configuration asset:
   - Go to `Tools > Bolt SDK > Configuration`
   - This opens the configuration window where you can set your `Game ID` and `Deep Link App Name`
   - Changes are automatically saved

2. Use the configuration in your code:

```csharp
using BoltSDK;
using UnityEngine;

public class BoltExample : MonoBehaviour
{
    private IBoltSDK boltSDK;
    
    void Start()
    {
        // Initialize the SDK - it will automatically load the configuration asset
        boltSDK = new BoltSDK.BoltSDK();
        boltSDK.Init(); // No parameters needed!
        
        // Subscribe to events
        boltSDK.onTransactionComplete += OnTransactionComplete;
        boltSDK.onCheckoutOpen += OnCheckoutOpen;
    }
    
    public void PurchaseItem(string checkoutLink)
    {
        if (boltSDK.IsInitialized)
        {
            boltSDK.OpenCheckout(checkoutLink);
        }
    }
    
    private void OnTransactionComplete(TransactionResult result)
    {
        Debug.Log($"Transaction completed: {result.Status}");
    }
    
    private void OnCheckoutOpen()
    {
        Debug.Log("Checkout opened");
    }
}
```

#### Option 2: Direct Initialization

```csharp
using BoltSDK;
using UnityEngine;

public class BoltExample : MonoBehaviour
{
    private IBoltSDK boltSDK;
    
    void Start()
    {
        // Initialize the SDK with your game ID and deep link app name
        boltSDK = new BoltSDK.BoltSDK();
        boltSDK.Init("your-game-id", "your-deep-link-app-name");
        
        // Subscribe to events
        boltSDK.onTransactionComplete += OnTransactionComplete;
        boltSDK.onCheckoutOpen += OnCheckoutOpen;
    }
    
    public void PurchaseItem(string productId)
    {
        if (boltSDK.IsInitialized)
        {
            boltSDK.OpenCheckout(productId);
        }
    }
    
    private void OnTransactionComplete(TransactionResult result)
    {
        Debug.Log($"Transaction completed: {result.Status}");
    }
    
    private void OnCheckoutOpen()
    {
        Debug.Log("Checkout opened");
    }
}
```

### Advanced Usage with Extra Parameters

```csharp
using BoltSDK;
using UnityEngine;
using System.Collections.Generic;

public class BoltAdvancedExample : MonoBehaviour
{
    private IBoltSDK boltSDK;
    
    void Start()
    {
        boltSDK = new BoltSDK.BoltSDK();
        boltSDK.Init(); // Automatically loads configuration
        
        // Load your checkout links from JSON or other sources
        LoadCheckoutLinks();
    }
    
    private void LoadCheckoutLinks()
    {
        // Load checkout links from JSON file
        TextAsset linksJson = Resources.Load<TextAsset>("checkout_links");
        if (linksJson != null)
        {
            // Parse checkout links JSON
            // Implementation depends on your data structure
        }
    }
    
    public void PurchaseWithExtraParams(string checkoutLink)
    {
        var extraParams = new Dictionary<string, string>
        {
            {"user_level", "10"},
            {"currency", "USD"},
            {"item_id", "premium_gems"}
        };
        
        boltSDK.OpenCheckout(checkoutLink, extraParams);
    }
}
```

## 📋 API Reference

### IBoltSDK Interface

The main interface for the Bolt Unity SDK:

```csharp
public interface IBoltSDK
{
    // Events
    event Action<TransactionResult> onTransactionComplete;
    event Action<void> onCheckoutOpen;

    // Status Properties
    bool IsInitialized { get; }
    BoltUser BoltUser { get; }
    string DeviceLocale { get; }
    string DeviceCountry { get; }

    // Initialization
    void Init(); // Automatically loads configuration asset
    void Init(string gameID, string deepLinkAppName = null);

    // Checkout Flow
    void OpenCheckout(string checkoutLink, Dictionary<string, string> extraParams = null);
    void HandleWeblinkCallback(string callbackUrl, Action<TransactionResult> onResult);
}
```

### Core Classes

#### BoltUser
```csharp
public class BoltUser
{
    public string Email { get; set; }
    public string Locale { get; set; }
    public string Country { get; set; }
    // Additional user properties
}
```

#### Catalog
```csharp
public class Catalog
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public string CheckoutUrl { get; set; }
}
```

#### TransactionResult
```csharp
public class TransactionResult
{
    public string TransactionId { get; set; }
    public TransactionStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime Timestamp { get; set; }
}
```

#### BoltAnalyticEvent
```csharp
public class BoltAnalyticEvent
{
    public string EventType { get; set; }
    public Dictionary<string, object> Properties { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## 🧪 Testing

The SDK includes comprehensive test coverage. Run tests using Unity's Test Runner:

1. Open Window > General > Test Runner
2. Select the test assembly
3. Run all tests or specific test categories

### Test Categories

- **Unit Tests**: Core functionality testing
- **Integration Tests**: End-to-end flow testing
- **Mock Tests**: Network and external service mocking

## 📁 Project Structure

```
Runtime/
├── Core/
│   ├── IBoltSDK.cs              # Main interface
│   ├── BoltSDK.cs               # Main implementation
│   └── BoltConfig.cs            # Configuration
├── Models/
│   ├── BoltUser.cs              # User data model
│   ├── Catalog.cs               # Product catalog
│   ├── TransactionResult.cs     # Transaction data
│   └── BoltAnalyticEvent.cs    # Analytics events
├── Services/
│   ├── IWebLinkService.cs       # web link abstraction
│   ├── UnityWebLinkService.cs   # Unity web link implementation
│   ├── IStorageService.cs       # Data storage abstraction
│   ├── PlayerPrefsStorageService.cs # Unity storage implementation
│   └── INetworkService.cs       # Network operations
├── Utils/
│   ├── JsonUtils.cs             # JSON utilities
│   ├── UrlUtils.cs              # URL handling
│   └── DeviceUtils.cs           # Device information
└── TransactionStatus.cs         # Transaction status enum

Tests/
├── Runtime/
│   ├── BoltSDKTests.cs          # Main SDK tests
│   ├── WebLinkServiceTests.cs   # web link tests
│   ├── StorageServiceTests.cs   # Storage tests
│   └── NetworkServiceTests.cs   # Network tests
└── EditMode/
    └── EditorTests.cs           # Editor-specific tests
```

## 🔧 Configuration

### Using Configuration Asset (Recommended)

The easiest way to configure the Bolt SDK is using the configuration asset:

1. **Create Configuration Asset:**
   - Go to `Tools > Bolt SDK > Configuration`
   - This opens the configuration window where you can set your settings

2. **Configure Settings:**
   - **Game ID**: Your unique game identifier from Bolt
   - **Deep Link App Name**: Your deep link app name for handling callbacks
   - **Environment**: Select Development, Staging, or Production

3. **Use in Code:**
   ```csharp
   void Start()
   {
       var boltSDK = new BoltSDK.BoltSDK();
       boltSDK.Init(); // Automatically loads the configuration!
   }
   ```

### Environment Setup

For different environments, you can create multiple configuration assets:

- `BoltSDKConfig_Dev.asset` - Development settings
- `BoltSDKConfig_Staging.asset` - Staging settings  
- `BoltSDKConfig_Prod.asset` - Production settings

Switch between them by changing the reference in your MonoBehaviour or by loading them dynamically:

```csharp
#if DEVELOPMENT_BUILD
    var config = Resources.Load<BoltSDKConfig>("BoltSDKConfig_Dev");
#elif UNITY_EDITOR
    var config = Resources.Load<BoltSDKConfig>("BoltSDKConfig_Dev");
#else
    var config = Resources.Load<BoltSDKConfig>("BoltSDKConfig_Prod");
#endif
```

### Catalog Management

Store your product catalog as JSON:

```json
{
  "products": [
    {
      "productId": "gems_100",
      "name": "100 Gems",
      "description": "Get 100 gems for your game",
      "price": 0.99,
      "currency": "USD",
      "checkoutUrl": "https://your-checkout-link.com"
    }
  ]
}
```

## 🚨 Error Handling

The SDK provides comprehensive error handling:

```csharp
try
{
    boltSDK.OpenCheckout("product_id");
}
catch (BoltSDKException ex)
{
    Debug.LogError($"Bolt SDK Error: {ex.Message}");
}
catch (Exception ex)
{
    Debug.LogError($"Unexpected error: {ex.Message}");
}
```

## 🔄 Best Practices

### 1. Initialization
- Always check `IsInitialized` before making calls
- Initialize early in your app lifecycle
- Handle initialization errors gracefully

### 2. Event Handling
- Subscribe to events in `Start()` or `Awake()`
- Unsubscribe in `OnDestroy()` to prevent memory leaks
- Use try-catch blocks in event handlers

### 3. Transaction Management
- Always acknowledge transactions after successful completion
- Implement retry logic for failed acknowledgments
- Store transaction state locally for offline scenarios

### 4. Testing
- Use mock services for unit testing
- Test all error scenarios
- Validate transaction flows end-to-end

## 🆘 Troubleshooting

### Common Issues

1. **SDK not initialized**
   - Ensure `Init()` is called before any other methods
   - Check that API key and game ID are valid

2. **web link not opening**
   - Verify internet connectivity
   - Check URL format and validity
   - Ensure proper platform permissions

3. **Transactions not completing**
   - Verify webhook configuration
   - Check server endpoint availability
   - Validate transaction acknowledgment

### Debug Mode

Enable debug logging:

```csharp
BoltSDK.EnableDebugMode(true);
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Need help?

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
