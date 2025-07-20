using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BoltSDK
{
    public class BoltSDKManager : MonoBehaviour, IBoltSDKManager
    {
        private static BoltSDKManager _instance;
        private readonly BoltSDKManagerCore _core = new();

        public static BoltSDKManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("BoltSDKManager");
                    _instance = go.AddComponent<BoltSDKManager>();
                    UnityEngine.Object.DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public bool IsInitialized => _core.IsInitialized;

        protected override void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public bool Initialize(BoltSDKConfig config)
        {
            bool result = _core.Initialize(config);
            if (result)
            {
                Debug.Log("BoltSDK: Initialized successfully");
            }
            else
            {
                Debug.LogError("BoltSDK: Invalid configuration provided");
            }
            return result;
        }

        public string GetLocale()
        {
            return _core.GetLocale();
        }

        public void SetLocale(string locale)
        {
            _core.SetLocale(locale);
        }

        public string GenerateTransactionId()
        {
            return _core.GenerateTransactionId();
        }

        public TransactionStatus CheckTransactionStatus(string transactionId)
        {
            return _core.CheckTransactionStatus(transactionId);
        }

        public void SaveTransactionStatus(string transactionId, TransactionStatus status)
        {
            _core.SaveTransactionStatus(transactionId, status);
        }

        public string BuildCheckoutUrl(string productId, string transactionId)
        {
            string url = _core.BuildCheckoutUrl(productId, transactionId);
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("BoltSDK: Failed to build checkout URL");
            }
            return url;
        }

        public bool HandleDeepLink(string url)
        {
            bool result = _core.HandleDeepLink(url);
            if (result)
            {
                Debug.Log($"BoltSDK: Deep link processed successfully");
            }
            return result;
        }

        public void OpenCheckout(string productId, Action<string> onTransactionIdGenerated = null, Action<TransactionStatus> onTransactionComplete = null)
        {
            if (!_core.IsInitialized)
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
    }
}