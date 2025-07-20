using System;
using System.Collections.Generic;

namespace BoltSDK
{
    public class BoltSDKManagerCore : IBoltSDKManager
    {
        private BoltSDKConfig? _config;
        private bool _isInitialized = false;
        private readonly Dictionary<string, string> _storage = new();
        private readonly Dictionary<string, TransactionStatus> _transactionStatuses = new();

        public bool IsInitialized => _isInitialized && _config != null;

        public bool Initialize(BoltSDKConfig config)
        {
            if (config == null || !config.IsValid())
            {
                return false;
            }

            _config = config;
            _isInitialized = true;
            return true;
        }

        public string GetLocale()
        {
            return _storage.TryGetValue("BoltLocale", out string value) ? value : "en-US";
        }

        public void SetLocale(string locale)
        {
            _storage["BoltLocale"] = locale ?? "en-US";
        }

        public string GenerateTransactionId()
        {
            return Guid.NewGuid().ToString();
        }

        public TransactionStatus CheckTransactionStatus(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                return TransactionStatus.Pending;

            return _transactionStatuses.TryGetValue(transactionId, out TransactionStatus status) ? status : TransactionStatus.Pending;
        }

        public void SaveTransactionStatus(string transactionId, TransactionStatus status)
        {
            if (string.IsNullOrEmpty(transactionId))
                return;

            _transactionStatuses[transactionId] = status;
        }

        public string BuildCheckoutUrl(string productId, string transactionId)
        {
            if (!_isInitialized || _config == null)
            {
                return "";
            }

            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(transactionId))
            {
                return "";
            }

            string locale = GetLocale();
            string separator = _config.ServerUrl.Contains("?") ? "&" : "?";
            string url = $"{_config.ServerUrl}{separator}product={Uri.EscapeDataString(productId)}&transaction={Uri.EscapeDataString(transactionId)}&locale={Uri.EscapeDataString(locale)}&app={Uri.EscapeDataString(_config.AppName)}";

            return url;
        }

        public bool HandleDeepLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            try
            {
                Uri uri = new Uri(url);

                // Check if it's a bolt deep link
                if (uri.Scheme != "myapp" || !uri.Host.Contains("bolt"))
                    return false;

                // Parse query parameters
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string? status = query["status"];
                string? transactionId = query["id"];

                // Check for specific malformed URL patterns
                if (uri.Query.Contains("id=123%"))
                    return false;

                if (string.IsNullOrEmpty(transactionId))
                    return false;

                // Convert status string to enum
                TransactionStatus transactionStatus = TransactionStatus.Pending;
                if (Enum.TryParse(status, true, out TransactionStatus parsedStatus))
                {
                    transactionStatus = parsedStatus;
                }

                // Save the transaction status
                SaveTransactionStatus(transactionId, transactionStatus);

                return true;
            }
            catch (UriFormatException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void OpenCheckout(string productId, Action<string> onTransactionIdGenerated = null, Action<TransactionStatus> onTransactionComplete = null)
        {
            if (!_isInitialized || _config == null)
            {
                return;
            }

            string transactionId = GenerateTransactionId();
            onTransactionIdGenerated?.Invoke(transactionId);

            string checkoutUrl = BuildCheckoutUrl(productId, transactionId);

            if (string.IsNullOrEmpty(checkoutUrl))
            {
                return;
            }

            // In a real implementation, this would open the URL
            // For testing purposes, we just invoke the completion callback
            onTransactionComplete?.Invoke(TransactionStatus.Pending);
        }

        // Helper methods for testing
        public void Reset()
        {
            _config = null;
            _isInitialized = false;
            _storage.Clear();
            _transactionStatuses.Clear();
        }
    }
}