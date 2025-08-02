using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoltApp
{
    public static class DeepLinkUtils
    {
        public static PaymentLinkSession ParsePaymentLinkSessionResult(Dictionary<string, string> parameters)
        {
            try
            {
                //  TODO - Use strong types for parameters
                var paymentLinkId = parameters.GetValueOrDefault("payment_link_id", "");
                var paymentLinkUrl = parameters.GetValueOrDefault("payment_link_url", "");
                var status = parameters.GetValueOrDefault("status", "");

                var status = ParsePaymentLinkStatus(status);
                return new PaymentLinkSession(paymentLinkId, paymentLinkUrl, status);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BoltSDK] Failed to parse transaction result: {ex.Message}");
                return new TransactionResult
                {
                    Status = TransactionStatus.Failed,
                    ErrorMessage = ex.Message
                };
            }
        }

        static PaymentLinkStatus ParsePaymentLinkStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return PaymentLinkStatus.Pending;

            var lowerStatus = status.ToLower();
            switch (lowerStatus)
            {
                case "completed":
                case "success":
                case "successful":
                    return PaymentLinkStatus.Completed;
                case "expired":
                    return PaymentLinkStatus.Expired;
                case "abandoned":
                case "cancelled":
                case "canceled":
                    return PaymentLinkStatus.Abandoned;
                default:
                    return PaymentLinkStatus.Pending;
            }
        }
    }
}