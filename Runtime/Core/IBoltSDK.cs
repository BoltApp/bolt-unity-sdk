using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoltApp
{
    /// <summary>
    /// Main interface for the Bolt Unity SDK
    /// </summary>
    public interface IBoltSDK
    {
        event Action<PaymentLinkSession> onTransactionComplete;
        event Action<PaymentLinkSession> onTransactionFailed;
        event Action onWebLinkOpen;
        event Action<string> onAdOpened;
        event Action<string> onAdCompleted;
        BoltConfig Config { get; }
        BoltUser GetBoltUser();
        BoltUser SetBoltUserData(string email = null, string locale = null, string country = null);
        void OpenCheckout(string checkoutLink);
        AdSession PreloadAd();
        AdSession ShowAd();
        PaymentLinkSession HandleDeepLinkCallback(string callbackUrl);
        PaymentLinkSession GetPaymentLinkSession(string paymentLinkId);
        PaymentLinkSession ResolvePaymentLinkSession(string paymentLinkId, PaymentLinkStatus status = PaymentLinkStatus.Successful, bool autoSave = true);
        Dictionary<string, PaymentLinkSession> GetPendingPaymentLinkSessions();
        bool HasPendingPaymentLinkSessions();
        void ManualSave();
    }
}