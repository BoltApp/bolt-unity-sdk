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
        public string PaymentLinkId;
        public string PaymentLinkUrl;
        public PaymentLinkStatus Status;
        public DateTime CreatedAt;
        public DateTime LastAccessedAt;
        public DateTime CompletedAt;

        public PaymentLinkSession(string paymentLinkId, string paymentLinkUrl, PaymentLinkStatus status = PaymentLinkStatus.Pending)
        {
            PaymentLinkId = paymentLinkId;
            PaymentLinkUrl = paymentLinkUrl;
            Status = status;
            CreatedAt = DateTime.UtcNow;
            LastAccessedAt = DateTime.UtcNow;
            if (status == PaymentLinkStatus.Successful)
            {
                CompletedAt = DateTime.UtcNow;
            }
        }

        public PaymentLinkSession(string paymentLinkId, string paymentLinkUrl, PaymentLinkStatus status, DateTime createdAt, DateTime lastAccessedAt, DateTime completedAt)
        {
            PaymentLinkId = paymentLinkId;
            PaymentLinkUrl = paymentLinkUrl;
            Status = status;
            CreatedAt = createdAt;
            LastAccessedAt = lastAccessedAt;
            CompletedAt = completedAt;
        }

        public void UpdateStatus(PaymentLinkStatus status)
        {
            Status = status;
            LastAccessedAt = DateTime.UtcNow;
            if (status == PaymentLinkStatus.Successful)
            {
                CompletedAt = DateTime.UtcNow;
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(PaymentLinkId) && !string.IsNullOrEmpty(PaymentLinkUrl);
        }

        public string ToString()
        {
            return $"PaymentLinkId: {PaymentLinkId}, Status: {Status}, CreatedAt: {CreatedAt}, LastAccessedAt: {LastAccessedAt}, CompletedAt: {CompletedAt}, PaymentLinkUrl: {PaymentLinkUrl}";
        }
    }
}