using System;
using System.Collections.Generic;

namespace Bolt.Unity.Utils
{
    public static class DeepLinkUtils
    {
        static TransactionResult ParseTransactionResult(Dictionary<string, string> parameters)
        {
            try
            {
                var transactionId = parameters.GetValueOrDefault("transaction_id", "");
                var status = parameters.GetValueOrDefault("status", "");
                var amount = parameters.GetValueOrDefault("amount", "");
                var currency = parameters.GetValueOrDefault("currency", "");
                var productId = parameters.GetValueOrDefault("product_id", "");

                return new TransactionResult
                {
                    TransactionId = transactionId,
                    Status = ParseTransactionStatus(status),
                    Amount = ParseAmount(amount),
                    Currency = currency,
                    ProductId = productId,
                    UserEmail = BoltUser?.Email,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                LogError($"Failed to parse transaction result: {ex.Message}");
                return new TransactionResult
                {
                    Status = TransactionStatus.Failed,
                    ErrorMessage = ex.Message
                };
            }
        }

        static TransactionStatus ParseTransactionStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return TransactionStatus.Pending;

            var lowerStatus = status.ToLower();
            switch (lowerStatus)
            {
                case "completed":
                case "success":
                case "successful":
                    return TransactionStatus.Completed;
                case "failed":
                case "error":
                    return TransactionStatus.Failed;
                case "cancelled":
                case "canceled":
                    return TransactionStatus.Cancelled;
                default:
                    return TransactionStatus.Pending;
            }
        }

        static decimal ParseAmount(string amount)
        {
            if (string.IsNullOrEmpty(amount))
                return 0m;

            if (decimal.TryParse(amount, out var result))
                return result;

            return 0m;
        }
    }
}