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
        public event Action<void> onCheckoutOpen;

        // Properties
        public bool IsInitialized { get; private set; }
        public BoltUser boltUser { get; private set; }
        private BoltConfig _config;

        // Configuration
        private string _gameId;
        private string _deepLinkAppName;

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
            _storageService = storageService ?? new PlayerPrefsStorageService();

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
        /// Loads the BoltSDKConfig asset from the default path
        /// </summary>
        /// <returns>The configuration asset or null if not found</returns>
        private BoltSDKConfig LoadConfigurationAsset()
        {
            // Try to load from the default path
            var config = Resources.Load<BoltSDKConfig>("BoltSDKConfig");

            if (config == null)
            {
                // Try to load from the Assets folder path
#if UNITY_EDITOR
                config = UnityEditor.AssetDatabase.LoadAssetAtPath<BoltSDKConfig>("Assets/BoltSDKConfig.asset");
#endif
            }

            return config;
        }

        public void Init(string gameID, string deepLinkAppName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(gameID))
                    throw new BoltSDKException("Game ID cannot be null or empty");

                _gameId = gameID;
                _deepLinkAppName = deepLinkAppName;
                _storageService.SetString("gameId", gameID);

                if (!string.IsNullOrEmpty(deepLinkAppName))
                {
                    _storageService.SetString("deepLinkAppName", deepLinkAppName);
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
        /// <param name="config">The BoltSDKConfig ScriptableObject containing game settings</param>
        public void Init(BoltSDKConfig config)
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

                onCheckoutOpen?.Invoke();
                _WebLinkService.OpenWebLink(checkoutLink, finalParams);
                LogDebug($"Opened checkout for product: {checkoutLink}");
            }
            catch (Exception ex)
            {
                LogError($"Failed to open checkout for product '{productId}': {ex.Message}");
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
                var transactionResult = ParseTransactionResult(queryParameters);

                StoreTransaction(transactionResult);
                onTransactionComplete?.Invoke(transactionResult);

                LogDebug($"Handled weblink callback for transaction: {transactionResult.TransactionId}");
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

        public string[] GetUnacknowledgedTransactions()
        {
            try
            {
                return _pendingTransactions.ToArray();
            }
            catch (Exception ex)
            {
                LogError($"Failed to get unacknowledged transactions: {ex.Message}");
                return new string[0];
            }
        }

        public bool AcknowledgeTransactions(string[] transactionRefIDs)
        {
            try
            {
                if (transactionRefIDs == null || transactionRefIDs.Length == 0)
                    return true;

                var successCount = 0;
                foreach (var transactionId in transactionRefIDs)
                {
                    if (string.IsNullOrEmpty(transactionId))
                        continue;

                    if (_pendingTransactions.Contains(transactionId))
                    {
                        _pendingTransactions.Remove(transactionId);
                        _acknowledgedTransactions.Add(transactionId);
                        successCount++;
                    }
                }

                SavePendingTransactions();
                SaveAcknowledgedTransactions();
                LogDebug($"Acknowledged {successCount} out of {transactionRefIDs.Length} transactions");
                return successCount == transactionRefIDs.Length;
            }
            catch (Exception ex)
            {
                LogError($"Failed to acknowledge transactions: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Enables or disables debug mode
        /// </summary>
        /// <param name="enabled">Whether debug mode should be enabled</param>
        public static void EnableDebugMode(bool enabled)
        {
            // This would be implemented as a static setting
            // For now, we'll use a simple approach
            Debug.unityLogger.logEnabled = enabled;
        }

        #region Private Methods

        private void InitializeUserData()
        {
            try
            {
                var existingUser = _storageService.GetObject<BoltUser>("userData");
                if (existingUser != null)
                {
                    boltUser = existingUser;
                    boltUser.LastActive = DateTime.UtcNow;
                }
                else
                {
                    boltUser = new BoltUser
                    {
                        Email = _storageService.GetString("userEmail", ""),
                        Locale = DeviceUtils.GetDeviceLocale(),
                        Country = DeviceUtils.GetDeviceCountry(),
                        DeviceId = DeviceUtils.GetDeviceId()
                    };
                }

                _storageService.SetObject("userData", boltUser);
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

        private TransactionResult CreatePendingTransaction(string productId, float price, string currency)
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

                var pendingTransactions = GetPendingTransactions();
                pendingTransactions.Add(pendingTransaction);
                _storageService.SetObject("pendingTransactions", pendingTransactions);
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
                var pendingData = _storageService.GetString("pendingTransactions", "");
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

        private void SavePendingTransactions(TransactionResult[] pendingTransactions)
        {
            try
            {
                var json = JsonUtils.ToJson(_pendingTransactions.ToArray());
                _storageService.SetString("pendingTransactions", json);
            }
            catch (Exception ex)
            {
                LogError($"Failed to save pending transactions: {ex.Message}");
            }
        }

        private void StoreTransaction(TransactionResult transactionResult)
        {
            try
            {
                if (string.IsNullOrEmpty(transactionResult.TransactionId))
                    return;

                // Add to pending transactions if not already acknowledged
                if (!_acknowledgedTransactions.Contains(transactionResult.TransactionId))
                {
                    if (!_pendingTransactions.Contains(transactionResult.TransactionId))
                    {
                        _pendingTransactions.Add(transactionResult.TransactionId);
                        SavePendingTransactions();
                    }
                }

                // Store the transaction result
                var transactionKey = $"transaction_{transactionResult.TransactionId}";
                _storageService.SetObject(transactionKey, transactionResult);
            }
            catch (Exception ex)
            {
                LogError($"Failed to store transaction: {ex.Message}");
            }
        }

        private TransactionResult ParseTransactionResult(Dictionary<string, string> parameters)
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

        private TransactionStatus ParseTransactionStatus(string status)
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

        private decimal ParseAmount(string amount)
        {
            if (string.IsNullOrEmpty(amount))
                return 0m;

            if (decimal.TryParse(amount, out var result))
                return result;

            return 0m;
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

                // Save any pending data
                SavePendingTransactions();
                SaveAcknowledgedTransactions();

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