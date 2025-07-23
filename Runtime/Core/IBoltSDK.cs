using System;
using System.Collections.Generic;

namespace BoltSDK
{
    /// <summary>
    /// Main interface for the Bolt Unity SDK
    /// </summary>
    public interface IBoltSDK
    {
        event Action<TransactionResult> onTransactionComplete;
        event Action<TransactionResult> onTransactionFailed;
        event Action onWebLinkOpen;

        bool IsInitialized { get; }
        BoltUser User { get; }

        void Init(BoltConfig config);
        void OpenCheckout(string checkoutLink, Dictionary<string, string> extraParams = null);
        TransactionResult HandleWeblinkCallback(string callbackUrl);
    }
}