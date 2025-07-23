using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoltSDK
{
    /// <summary>
    /// Main implementation of the Bolt Unity SDK
    /// </summary>
    public class BoltSDK : IBoltSDK
    {
        // Callback Events
        public event Action<TransactionResult> onTransactionComplete;
        public event Action<TransactionResult> onTransactionFailed;
        public event Action onWebLinkOpen;

        // Properties
        public bool IsInitialized { get; private set; }
        public BoltUser User { get; private set; }

        // Configuration
        public string GameID { get; private set; }
        public string DeepLinkAppName { get; private set; }

        // Services
        private IWebLinkService _WebLinkService;
        private IStorageService _StorageService;

        public BoltSDK()
        {
            // Initialize default services
            _WebLinkService = new UnityWebLinkService();
            _StorageService = new PlayerPrefsStorageService();

            // Set up event handlers
            _WebLinkService.OnWebLinkOpened += OnWebLinkOpened;
            _WebLinkService.OnWebLinkClosed += OnWebLinkClosed;
            _WebLinkService.OnError += OnWebLinkError;
        }

        public BoltSDK(IWebLinkService WebLinkService, IStorageService storageService)
        {
            _WebLinkService = WebLinkService ?? new UnityWebLinkService();
            _StorageService = storageService ?? new PlayerPrefsStorageService();

            // Set up event handlers
            _WebLinkService.OnWebLinkOpened += OnWebLinkOpened;
            _WebLinkService.OnWebLinkClosed += OnWebLinkClosed;
            _WebLinkService.OnError += OnWebLinkError;
        }

        public void Init()
        {
            // Load configuration from the asset
            var config = LoadConfigurationAsset();

            if (config == null)
            {
                throw new BoltSDKException("No Bolt SDK Configuration found. Please create one using Tools > Bolt SDK > Configuration");
            }

            if (!config.IsValid())
            {
                throw new BoltSDKException("Bolt SDK Configuration is not valid. Please check the configuration using Tools > Bolt SDK > Configuration");
            }

            Init(config.gameId, config.deepLinkAppName);
        }

        /// <summary>
        /// Loads the BoltConfig asset from the default path
        /// </summary>
        /// <returns>The configuration asset or null if not found</returns>
        private BoltConfig LoadConfigurationAsset()
        {
            // Try to load from the default path
            var config = Resources.Load<BoltConfig>("BoltConfig");

            if (config == null)
            {
                // Try to load from the Assets folder path
#if UNITY_EDITOR
                config = UnityEditor.AssetDatabase.LoadAssetAtPath<BoltConfig>("Assets/BoltConfig.asset");
#endif
            }

            return config;
        }

        public void Init(BoltConfig config)
        {
            try
            {
                if (string.IsNullOrEmpty(config.gameId))
                    throw new BoltSDKException("Game ID cannot be null or empty");

                GameID = config.gameId;
                DeepLinkAppName = config.deepLinkAppName;

                if (!string.IsNullOrEmpty(config.deepLinkAppName))
                {
                    _StorageService.SetString("deepLinkAppName", config.deepLinkAppName);
                }

                InitializeUserData();
                IsInitialized = true;

                LogDebug($"Bolt SDK initialized successfully. Game ID: {gameID}");
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize Bolt SDK: {ex.Message}");
                throw new BoltSDKException($"Failed to initialize Bolt SDK: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Initialize the SDK using a configuration asset
        /// </summary>
        /// <param name="config">The BoltConfig ScriptableObject containing game settings</param>
        public void Init(BoltConfig config)
        {
            if (config == null)
                throw new BoltSDKException("Configuration cannot be null");

            if (!config.IsValid())
                throw new BoltSDKException("Configuration is not valid. Please check the error messages above.");

            Init(config.gameId, config.deepLinkAppName);
        }

        public void OpenCheckout(string checkoutLink, Dictionary<string, string> extraParams = null)
        {
            try
            {
                if (!IsInitialized)
                    throw new BoltSDKNotInitializedException();

                if (string.IsNullOrEmpty(checkoutLink))
                    throw new BoltSDKException("Checkout link cannot be null or empty");

                var finalParams = new Dictionary<string, string>
                {
                    { "device_locale", DeviceUtils.GetDeviceLocale() },
                    { "user_email", BoltUser?.Email ?? "" },
                    { "device_country", DeviceUtils.GetDeviceCountry() },
                    { "app_name", Application.productName },
                    { "device_id", DeviceUtils.GetDeviceId() }
                };

                if (extraParams != null)
                {
                    foreach (var param in extraParams)
                    {
                        finalParams[param.Key] = param.Value;
                    }
                }

                onWebLinkOpen?.Invoke();
                _WebLinkService.OpenWebLink(checkoutLink, finalParams);
                LogDebug($"Opened checkout for product: {checkoutLink}");
            }
            catch (Exception ex)
            {
                LogError($"Failed to open checkout for product '{checkoutLink}': {ex.Message}");
                throw;
            }
        }

        public TransactionResult HandleWeblinkCallback(string callbackUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(callbackUrl))
                {
                    LogError("Callback URL cannot be null or empty");
                    return new TransactionResult
                    {
                        Status = TransactionStatus.Failed,
                        ErrorMessage = "Failed to parse transaction. Callback URL cannot be null or empty",
                        TransactionId = ""
                    };
                }

                var queryParameters = UrlUtils.ExtractQueryParameters(callbackUrl);
                var transactionResult = DeepLinkUtils.ParseTransactionResult(queryParameters);

                StoreTransaction(transactionResult);
                onTransactionComplete?.Invoke(transactionResult);

                LogDebug($"Handled weblink callback for transaction: {transactionResult.TransactionId}");
                return transactionResult;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to handle weblink callback: {ex.Message}";
                LogError(errorMessage);
                return new TransactionResult
                {
                    Status = TransactionStatus.Failed,
                    ErrorMessage = errorMessage,
                    TransactionId = ""
                };
            }
        }

        #region Private Methods

        private void InitializeUserData()
        {
            try
            {
                var existingUser = _StorageService.GetObject<BoltUser>("userData");
                if (existingUser != null)
                {
                    boltUser = existingUser;
                    boltUser.LastActive = DateTime.UtcNow;
                }
                else
                {
                    boltUser = new BoltUser
                    {
                        Email = _StorageService.GetString("userEmail", ""),
                        Locale = DeviceUtils.GetDeviceLocale(),
                        Country = DeviceUtils.GetDeviceCountry(),
                        DeviceId = DeviceUtils.GetDeviceId()
                    };
                }

                _StorageService.SetObject("userData", boltUser);
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize user data: {ex.Message}");
                boltUser = new BoltUser
                {
                    Locale = DeviceUtils.GetDeviceLocale(),
                    Country = DeviceUtils.GetDeviceCountry(),
                    DeviceId = DeviceUtils.GetDeviceId()
                };
            }
        }

        private TransactionResult CreateNewTransaction(string productId, float price, string currency)
        {
            try
            {
                var pendingTransaction = new TransactionResult
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Status = TransactionStatus.Pending,
                    Amount = price,
                    Currency = currency,
                    ProductId = productId,
                    UserEmail = boltUser.Email,
                    Timestamp = DateTime.UtcNow
                };

                SavePendingTransaction(pendingTransaction);
                return pendingTransaction;
            }
            catch (Exception ex)
            {
                LogError($"Failed to load pending transactions: {ex.Message}");
                _pendingTransactions = new List<string>();
            }
        }

        private TransactionResult[] GetPendingTransactions()
        {
            try
            {
                var pendingData = _StorageService.GetString("pendingTransactions", "");
                if (!string.IsNullOrEmpty(pendingData))
                {
                    var transactions = JsonUtils.FromJson<TransactionResult[]>(pendingData);
                    return transactions ?? new TransactionResult[0];
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to load pending transactions: {ex.Message}");
                return new TransactionResult[0];
            }
        }

        private void SavePendingTransaction(TransactionResult transactionResult)
        {
            try
            {
                var pendingTransactions = GetPendingTransactions();

                // Find the transaction in the list and update it
                var existingTransaction = pendingTransactions.FirstOrDefault(t => t.TransactionId == transactionResult.TransactionId);
                if (existingTransaction != null)
                {
                    existingTransaction = transactionResult;
                }
                else
                {
                    Debug.Log($"Failed to find transaction in pending transactions: {transactionResult.TransactionId}");
                    pendingTransactions.Add(transactionResult);
                }

                var json = JsonUtils.ToJson(pendingTransactions);
                _StorageService.SetString("pendingTransactions", json);
            }
            catch (Exception ex)
            {
                LogError($"Failed to save pending transactions: {ex.Message}");
            }
        }

        #endregion

        #region Event Handlers

        private void OnWebLinkOpened()
        {
            LogDebug("web link opened");
        }

        private void OnWebLinkClosed()
        {
            LogDebug("web link closed");
        }

        private void OnWebLinkError(string error)
        {
            LogError($"web link error: {error}");
        }

        #endregion

        #region Logging

        private void LogDebug(string message)
        {
            if (_debugMode)
            {
                Debug.Log($"[BoltSDK] {message}");
            }
        }

        private void LogError(string message)
        {
            Debug.LogError($"[BoltSDK] {message}");
        }

        #endregion

        #region Cleanup

        public void Dispose()
        {
            try
            {
                // Unsubscribe from events
                if (_WebLinkService != null)
                {
                    _WebLinkService.OnWebLinkOpened -= OnWebLinkOpened;
                    _WebLinkService.OnWebLinkClosed -= OnWebLinkClosed;
                    _WebLinkService.OnError -= OnWebLinkError;
                }

                IsInitialized = false;
            }
            catch (Exception ex)
            {
                LogError($"Error during disposal: {ex.Message}");
            }
        }

        #endregion
    }
}