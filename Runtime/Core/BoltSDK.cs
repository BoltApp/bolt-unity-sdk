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
        public event Action<TransactionResult> onTransactionComplete;
        public event Action<TransactionResult> onTransactionFailed;
        public event Action onWebLinkOpen;
        public BoltConfig Config { get; private set; }
        private IStorageService _StorageService;

        public BoltSDK()
        {
            Config = new BoltConfig();
            _StorageService = new PlayerPrefsStorageService();
        }

        public BoltSDK(BoltConfig config)
        {
            Config = config;
            _StorageService = new PlayerPrefsStorageService();
        }

        public void OpenCheckout(string checkoutLink, Dictionary<string, string> extraParams = null)
        {
            try
            {
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