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
        public PaymentLinkStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessedAt { get; set; }
        public DateTime CompletedAt { get; set; }

        public PaymentLinkSession(string paymentLinkId, string paymentLinkUrl, PaymentLinkStatus status = PaymentLinkStatus.Pending)
        {
            PaymentLinkId = paymentLinkId;
            PaymentLinkUrl = paymentLinkUrl;
            Status = status;
            CreatedAt = DateTime.UtcNow;
            LastAccessedAt = CreatedAt;
            if (status == PaymentLinkStatus.Completed)
            {
                CompletedAt = LastAccessedAt;
            }
        }

        public void UpdateStatus(PaymentLinkStatus status)
        {
            Status = status;
            LastAccessedAt = DateTime.UtcNow;
            if (status == PaymentLinkStatus.Completed)
            {
                CompletedAt = LastAccessedAt;
            }
        }
    }
}