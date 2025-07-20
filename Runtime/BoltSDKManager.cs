using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BoltSDK
{
    public class BoltSDKManager : MonoBehaviour
    {
        private static BoltSDKManager _instance;
        private BoltSDKConfig _config;
        private bool _isInitialized = false;

        public static BoltSDKManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("BoltSDKManager");
                    _instance = go.AddComponent<BoltSDKManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public bool IsInitialized => _isInitialized;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public bool Initialize(BoltSDKConfig config)
        {
            if (!config.IsValid())
            {
                Debug.LogError("BoltSDK: Invalid configuration provided");
                return false;
            }

            _config = config;
            _isInitialized = true;
            Debug.Log("BoltSDK: Initialized successfully");
            return true;
        }

        public string GetLocale()
        {
            return PlayerPrefs.GetString("BoltLocale", "en-US");
        }

        public void SetLocale(string locale)
        {
            PlayerPrefs.SetString("BoltLocale", locale);
            PlayerPrefs.Save();
        }

        public string GenerateTransactionId()
        {
            return Guid.NewGuid().ToString();
        }

        public TransactionStatus CheckTransactionStatus(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                return TransactionStatus.Pending;

            string status = PlayerPrefs.GetString($"BoltTransaction_{transactionId}", "");

            switch (status.ToLower())
            {
                case "completed":
                    return TransactionStatus.Completed;
                case "failed":
                    return TransactionStatus.Failed;
                case "cancelled":
                    return TransactionStatus.Cancelled;
                default:
                    return TransactionStatus.Pending;
            }
        }

        public void SaveTransactionStatus(string transactionId, TransactionStatus status)
        {
            if (string.IsNullOrEmpty(transactionId))
                return;

            string statusString = status.ToString().ToLower();
            PlayerPrefs.SetString($"BoltTransaction_{transactionId}", statusString);
            PlayerPrefs.Save();
        }

        public string BuildCheckoutUrl(string productId, string transactionId)
        {
            if (!_isInitialized)
            {
                Debug.LogError("BoltSDK: Not initialized. Call Initialize() first.");
                return "";
            }

            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(transactionId))
            {
                Debug.LogError("BoltSDK: Product ID and Transaction ID are required");
                return "";
            }

            string locale = GetLocale();
            string url = $"{_config.ServerUrl}/checkout?product={Uri.EscapeDataString(productId)}&transaction={Uri.EscapeDataString(transactionId)}&locale={Uri.EscapeDataString(locale)}&app={Uri.EscapeDataString(_config.AppName)}";

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
                string status = query["status"];
                string transactionId = query["id"];

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

                Debug.Log($"BoltSDK: Deep link processed - Transaction {transactionId} status: {transactionStatus}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"BoltSDK: Error processing deep link: {ex.Message}");
                return false;
            }
        }

        public void OpenCheckout(string productId, Action<string> onTransactionIdGenerated = null, Action<TransactionStatus> onTransactionComplete = null)
        {
            if (!_isInitialized)
            {
                Debug.LogError("BoltSDK: Not initialized. Call Initialize() first.");
                return;
            }

            string transactionId = GenerateTransactionId();
            onTransactionIdGenerated?.Invoke(transactionId);

            string checkoutUrl = BuildCheckoutUrl(productId, transactionId);

            if (string.IsNullOrEmpty(checkoutUrl))
            {
                Debug.LogError("BoltSDK: Failed to build checkout URL");
                return;
            }

            // Open the checkout URL in the default browser
            Application.OpenURL(checkoutUrl);

            // Start monitoring for transaction completion
            StartCoroutine(MonitorTransaction(transactionId, onTransactionComplete));
        }

        private IEnumerator MonitorTransaction(string transactionId, Action<TransactionStatus> onComplete)
        {
            float checkInterval = 1.0f; // Check every second
            float timeout = 300.0f; // 5 minutes timeout
            float elapsed = 0f;

            while (elapsed < timeout)
            {
                TransactionStatus status = CheckTransactionStatus(transactionId);

                if (status != TransactionStatus.Pending)
                {
                    onComplete?.Invoke(status);
                    yield break;
                }

                yield return new WaitForSeconds(checkInterval);
                elapsed += checkInterval;
            }

            // Timeout reached
            onComplete?.Invoke(TransactionStatus.Failed);
        }
    }
}