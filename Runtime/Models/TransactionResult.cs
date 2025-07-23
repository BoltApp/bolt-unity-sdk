using System;
using System.Collections.Generic;

namespace BoltSDK
{
    /// <summary>
    /// Represents the result of a Bolt transaction
    /// </summary>
    [Serializable]
    public class TransactionResult
    {
        public string TransactionId { get; set; }
        public TransactionStatus Status { get; set; }
        public bool IsServerVerified { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Timestamp { get; set; }
        public string ProductId { get; set; }
        public string UserEmail { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public TransactionResult()
        {
            Timestamp = DateTime.UtcNow;
            Metadata = new Dictionary<string, object>();
        }

        public TransactionResult(string transactionId, TransactionStatus status, decimal amount, string currency)
        {
            TransactionId = transactionId;
            Status = status;
            Amount = amount;
            Currency = currency;
            Timestamp = DateTime.UtcNow;
            Metadata = new Dictionary<string, object>();
        }

        public bool IsSuccessful => Status == TransactionStatus.Completed;
        public bool IsFailed => Status == TransactionStatus.Failed;
        public bool IsPending => Status == TransactionStatus.Pending;
        public bool IsCancelled => Status == TransactionStatus.Cancelled;
        public bool IsServerVerified => IsServerVerified;
    }
}