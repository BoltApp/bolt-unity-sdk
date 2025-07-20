using System;

namespace BoltSDK
{
    public interface IBoltSDKManager
    {
        bool IsInitialized { get; }
        bool Initialize(BoltSDKConfig config);
        string GetLocale();
        void SetLocale(string locale);
        string GenerateTransactionId();
        TransactionStatus CheckTransactionStatus(string transactionId);
        void SaveTransactionStatus(string transactionId, TransactionStatus status);
        string BuildCheckoutUrl(string productId, string transactionId);
        bool HandleDeepLink(string url);
        void OpenCheckout(string productId, Action<string> onTransactionIdGenerated = null, Action<TransactionStatus> onTransactionComplete = null);
    }
}