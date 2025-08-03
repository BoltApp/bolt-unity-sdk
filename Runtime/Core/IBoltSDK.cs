using System;
using System.Collections.Generic;

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
        BoltConfig Config { get; }
        BoltUser GetBoltUser();
        BoltUser SetBoltUserData(string email = null, string locale = null, string country = null);
        void OpenCheckout(string checkoutLink);
        PaymentLinkSession HandleDeepLinkCallback(string callbackUrl);
        PaymentLinkSession GetPaymentLinkSession(string paymentLinkId);
        PaymentLinkSession ResolvePaymentLinkSession(string paymentLinkId, PaymentLinkStatus status = PaymentLinkStatus.Successful);
        List<PaymentLinkSession> GetPaymentLinkSessionHistory();
        List<PaymentLinkSession> GetPendingPaymentLinkSessions();
    }
}