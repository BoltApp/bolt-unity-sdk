using System;
using System.Collections.Generic;

namespace BoltApp
{
    /// <summary>
    /// Main interface for the Bolt Unity SDK
    /// </summary>
    public interface IBoltSDK
    {
        event Action<PendingPaymentLink> onTransactionComplete;
        event Action<PendingPaymentLink> onTransactionFailed;
        event Action onWebLinkOpen;
        BoltConfig Config { get; }
        BoltUser GetBoltUser();
        BoltUser SetBoltUserData(string email = null, string locale = null, string country = null);
        void OpenCheckout(string checkoutLink);
        void HandleDeepLinkCallback(string callbackUrl);
        List<PaymentSession> GetPendingPaymentSessions();
        PaymentSession ResolvePaymentSession(string paymentLinkId, PaymentLinkStatus status);
        List<PaymentSession> GetDevicePaymentSessionHistory();

    }
}