using System;
using System.Collections.Generic;

namespace BoltApp
{
    /// <summary>
    /// Wrapper class for serializing List<PaymentLinkSession> with Unity's JsonUtility
    /// </summary>
    [Serializable]
    public class PaymentLinkHistory
    {
        public List<PaymentLinkSession> sessions;

        public PaymentLinkHistory()
        {
            sessions = new List<PaymentLinkSession>();
        }

        public PaymentLinkHistory(List<PaymentLinkSession> paymentLinkSessions)
        {
            sessions = paymentLinkSessions ?? new List<PaymentLinkSession>();
        }
    }
}