using System;
using System.Collections.Generic;

namespace BoltApp
{
    /// <summary>
    /// Main interface for the Bolt Unity SDK
    /// </summary>
    public interface IBoltSDK
    {
        event Action<TransactionResult> onTransactionComplete;
        event Action<TransactionResult> onTransactionFailed;
        event Action onWebLinkOpen;
        BoltConfig Config { get; }
        void OpenCheckout(string checkoutLink, Dictionary<string, string> extraParams = null);
        void CancelTransaction(string transactionId, bool serverValidated = false);
        void CompleteTransaction(string transactionId, bool serverValidated = false);
        List<TransactionResult> GetTransactions();
        TransactionResult HandleDeepLinkCallback(string callbackUrl);
    }
}