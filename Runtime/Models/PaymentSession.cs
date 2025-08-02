using System;
using System.Collections.Generic;

namespace BoltApp
{
    /// <summary>
    /// Light wrapper around a payment link object used in tracking recent checkouts
    /// </summary>
    [Serializable]
    public class PaymentLinkSession
    {
        public string PaymentLinkId { get; set; }
        public string PaymentLinkUrl { get; set; }
        public string Status { get; set; }
        public string StartedAt { get; set; }
        public string LastAccessedAt { get; set; }
        public string CompletedAt { get; set; }

        public PaymentLinkSession(string paymentLinkId, string paymentLinkUrl)
        {
            PaymentLinkId = paymentLinkId;
            PaymentLinkUrl = paymentLinkUrl;
            StartedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            LastAccessedAt = StartedAt;
        }

        public void UpdateLastAccessedAt()
        {
            LastAccessedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void UpdateStatus(PaymentLinkStatus status)
        {
            Status = status;
            LastAccessedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            if (status == PaymentLinkStatus.Completed)
            {
                CompletedAt = LastAccessedAt;
            }
        }
    }
}