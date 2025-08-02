using System;
using System.Collections.Generic;

namespace BoltApp
{
    /// <summary>
    /// Main interface for the Bolt Unity SDK
    /// </summary>
    public interface IBoltSDK
    {
        event Action<PaymentSession> onTransactionComplete;
        event Action<PaymentSession> onTransactionFailed;
        event Action onWebLinkOpen;
        BoltConfig Config { get; }
        BoltUser GetBoltUser();
        BoltUser SetBoltUserData(string email = null, string locale = null, string country = null);
        void OpenCheckout(string checkoutLink);
        void HandleDeepLinkCallback(string callbackUrl);
        PaymentSession GetPaymentSession(string paymentLinkId);
        PaymentSession ResolvePaymentSession(string paymentLinkId, PaymentLinkStatus status = PaymentLinkStatus.Completed);
        List<PaymentSession> GetPaymentSessionHistory();
        List<PaymentSession> GetPendingPaymentSessions();
    }
}