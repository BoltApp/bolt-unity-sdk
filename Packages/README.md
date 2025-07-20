# Bolt SDK for Unity

A Unity SDK for performing in-app purchases and subscriptions outside of the app store.

## Features

- **Easy Integration**: Simple API for handling in-app purchases
- **Transaction Tracking**: Generate unique transaction IDs and track their status
- **Locale Support**: Built-in locale management with PlayerPrefs integration
- **Deep Link Handling**: Process transaction results via deep links
- **Editor Configuration**: Unity Editor popup for easy SDK configuration
- **Unit Tests**: Comprehensive test coverage for all functionality

## Installation

### Via Git (Recommended)

1. Add this package to your Unity project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.bolt.sdk": "git://https://github.com/your-repo/bolt-unity-sdk.git"
  }
}
```

### Via Unity Package Manager

1. Open Unity Package Manager
2. Click the "+" button
3. Select "Add package from git URL"
4. Enter: `https://github.com/your-repo/bolt-unity-sdk.git`

## Quick Start

### 1. Configure the SDK

1. In Unity, go to **Tools > Bolt SDK > Settings**
2. Enter your server URL (e.g., `https://api.bolt.com`)
3. Enter your app name
4. Click "Save Settings"

### 2. Initialize the SDK

```csharp
using BoltSDK;

// Create configuration
var config = new BoltSDKConfig("https://api.bolt.com", "YourAppName");

// Initialize the SDK
bool success = BoltSDKManager.Instance.Initialize(config);

if (success)
{
    Debug.Log("Bolt SDK initialized successfully!");
}
```

### 3. Start a Purchase

```csharp
// Start a purchase
BoltSDKManager.Instance.OpenCheckout(
    "product-id-123",
    onTransactionIdGenerated: (transactionId) =>
    {
        Debug.Log($"Transaction ID: {transactionId}");
    },
    onTransactionComplete: (status) =>
    {
        Debug.Log($"Transaction completed: {status}");
    }
);
```

### 4. Handle Deep Links

```csharp
// Handle deep links in your app's deep link handler
public void OnDeepLinkReceived(string url)
{
    bool processed = BoltSDKManager.Instance.HandleDeepLink(url);
    if (processed)
    {
        Debug.Log("Deep link processed successfully");
    }
}
```

## API Reference

### BoltSDKManager

The main SDK manager class that handles all Bolt SDK operations.

#### Properties

- `IsInitialized`: Returns whether the SDK has been initialized

#### Methods

- `Initialize(BoltSDKConfig config)`: Initialize the SDK with configuration
- `GetLocale()`: Get the current locale from PlayerPrefs
- `SetLocale(string locale)`: Set the locale and save to PlayerPrefs
- `GenerateTransactionId()`: Generate a unique transaction ID
- `CheckTransactionStatus(string transactionId)`: Check the status of a transaction
- `SaveTransactionStatus(string transactionId, TransactionStatus status)`: Save transaction status to PlayerPrefs
- `BuildCheckoutUrl(string productId, string transactionId)`: Build a checkout URL with parameters
- `HandleDeepLink(string url)`: Process deep links for transaction results
- `OpenCheckout(string productId, Action<string> onTransactionIdGenerated, Action<TransactionStatus> onTransactionComplete)`: Start a purchase flow

### BoltSDKConfig

Configuration class for the SDK.

#### Properties

- `ServerUrl`: The URL of your Bolt server
- `AppName`: The name of your application

#### Methods

- `IsValid()`: Check if the configuration is valid

### TransactionStatus

Enum for transaction status values:

- `Pending`: Transaction is pending
- `Completed`: Transaction completed successfully
- `Failed`: Transaction failed
- `Cancelled`: Transaction was cancelled

## Locale Management

The SDK automatically manages locale settings using PlayerPrefs:

```csharp
// Set locale
BoltSDKManager.Instance.SetLocale("fr-FR");

// Get current locale
string locale = BoltSDKManager.Instance.GetLocale();
```

## Transaction Tracking

The SDK provides comprehensive transaction tracking:

```csharp
// Generate a transaction ID
string transactionId = BoltSDKManager.Instance.GenerateTransactionId();

// Check transaction status
TransactionStatus status = BoltSDKManager.Instance.CheckTransactionStatus(transactionId);

// Save transaction status
BoltSDKManager.Instance.SaveTransactionStatus(transactionId, TransactionStatus.Completed);
```

## Deep Link Handling

The SDK can process deep links to handle transaction results:

```csharp
// Example deep link: myapp://bolt/transaction?status=completed&id=transaction-123
bool processed = BoltSDKManager.Instance.HandleDeepLink(url);
```

## Editor Configuration

Use the Unity Editor popup to configure the SDK:

1. Go to **Tools > Bolt SDK > Settings**
2. Enter your server URL and app name
3. Click "Save Settings"
4. Use "Test Configuration" to validate your settings

## Testing

Run the unit tests to verify SDK functionality:

1. Open Unity Test Runner (Window > General > Test Runner)
2. Select "EditMode" or "PlayMode" tests
3. Run the BoltSDK tests

## Sample Integration

See the `Samples~/BasicIntegration` folder for a complete example of how to integrate the SDK into your Unity project.

## Requirements

- Unity 2021.3 or later
- .NET 4.x or later

## License

MIT License - see LICENSE file for details.

## Support

For support, please contact support@bolt.com or visit https://bolt.com 