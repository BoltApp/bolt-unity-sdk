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
"com.yourorg.bolt-unity-sdk": "https://github.com/yourusername/bolt-unity-sdk.git"

```

## 🔧 Quick Start

**Requirements:** You must have a backend server for proxying API requests. See our [server sample](https://github.com/davidminin/bolt-gameserver-sample) for an example integration.

1. Add the SDK to your project
2. Add routes to your backend server [(see example usage)](https://github.com/davidminin/bolt-gameserver-sample/blob/main/example-usage.ts)
3. Use the staging api configs to test purchases in your dev environment

## 📚 Documentation

For detailed documentation and API reference, visit our [documentation site](https://docs.bolt.com).


## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
