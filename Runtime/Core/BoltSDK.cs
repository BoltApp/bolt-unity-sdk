using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoltApp
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

                BoltUser boltUser = GetUserData();

                var finalParams = new Dictionary<string, string>
                {
                    { "device_locale", DeviceUtils.GetDeviceLocale() },
                    { "user_email", boltUser.Email },
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

                LogDebug($"Opening checkout for product: {checkoutLink}");
                onWebLinkOpen?.Invoke();
                Application.OpenURL(checkoutLink);
            }
            catch (Exception ex)
            {
                LogError($"Failed to open checkout for product '{checkoutLink}': {ex.Message}");
                throw;
            }
        }

        public TransactionResult HandleDeepLinkCallback(string callbackUrl)
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

                if (Config.deepLinkAppName != null && !callbackUrl.Contains(Config.deepLinkAppName))
                {
                    // Skip deep link callback if it does not match the provided app name
                    LogDebug($"Deep link callback URL does not match config: {callbackUrl}. Expected: {Config.deepLinkAppName}");
                    return null;
                }

                var queryParameters = UrlUtils.ExtractQueryParameters(callbackUrl);
                var transactionResult = DeepLinkUtils.ParseTransactionResult(queryParameters);

                if (transactionResult.IsFailed)
                {
                    onTransactionFailed?.Invoke(transactionResult);
                    LogError($"Transaction failed: {transactionResult.ErrorMessage}");
                    return transactionResult;
                }

                CreateOrUpdateTransaction(transactionResult);
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

        private BoltUser GetUserData()
        {
            try
            {
                var existingUser = _StorageService.GetObject<BoltUser>("userData");
                if (existingUser != null)
                {
                    existingUser.LastActive = DateTime.UtcNow;
                    return existingUser;
                }
                else
                {
                    var newUser = new BoltUser
                    {
                        Email = _StorageService.GetString("userEmail", ""),
                        Locale = DeviceUtils.GetDeviceLocale(),
                        Country = DeviceUtils.GetDeviceCountry(),
                        DeviceId = DeviceUtils.GetDeviceId()
                    };
                    _StorageService.SetObject("userData", newUser);
                    return newUser;
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize user data: {ex.Message}");
                return new BoltUser
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
                BoltUser boltUser = GetUserData();
                var pendingTransaction = new TransactionResult
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Status = TransactionStatus.Pending,
                    Amount = (decimal)price,
                    Currency = currency,
                    ProductId = productId,
                    UserEmail = boltUser.Email,
                    IsServerValidated = false,
                    Timestamp = DateTime.UtcNow
                };

                CreateOrUpdateTransaction(pendingTransaction);
                return pendingTransaction;
            }
            catch (Exception ex)
            {
                LogError($"Failed to create new transaction: {ex.Message}");
                return null;
            }
        }

        public List<TransactionResult> GetTransactions()
        {
            try
            {
                var historyData = _StorageService.GetString("transactionHistory", "");
                if (string.IsNullOrEmpty(historyData))
                {
                    return new List<TransactionResult>();
                }

                var transactions = JsonUtility.FromJson<List<TransactionResult>>(historyData);
                return transactions ?? new List<TransactionResult>();
            }
            catch (Exception ex)
            {
                LogError($"Failed to load pending transactions: {ex.Message}");
                return new List<TransactionResult>();
            }
        }

        public List<TransactionResult> GetPendingTransactions()
        {
            var transactions = GetTransactions();
            return transactions.Where(t => t.Status == TransactionStatus.Pending).ToList();
        }

        public void CancelTransaction(string transactionId, bool serverValidated = false)
        {
            try
            {
                var transactionHistory = GetTransactions();
                var transaction = transactionHistory.FirstOrDefault(t => t.TransactionId == transactionId);
                if (transaction != null)
                {
                    transaction.Status = TransactionStatus.Cancelled;
                    transaction.IsServerValidated = serverValidated;
                    CreateOrUpdateTransaction(transaction);
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to cancel transaction: {ex.Message}");
            }
        }

        public void CompleteTransaction(string transactionId, bool serverValidated = false)
        {
            try
            {
                var transactionHistory = GetTransactions();
                var transaction = transactionHistory.FirstOrDefault(t => t.TransactionId == transactionId);
                if (transaction != null)
                {
                    transaction.Status = TransactionStatus.Completed;
                    transaction.IsServerValidated = serverValidated;
                    CreateOrUpdateTransaction(transaction);
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to complete transaction: {ex.Message}");
            }
        }

        private void CreateOrUpdateTransaction(TransactionResult transactionResult)
        {
            try
            {
                var transactionHistory = GetTransactions();

                // Find the transaction in the list and update it
                var existingTransaction = transactionHistory.FirstOrDefault(t => t.TransactionId == transactionResult.TransactionId);
                if (existingTransaction != null)
                {
                    existingTransaction = transactionResult;
                }
                else
                {
                    transactionHistory.Add(transactionResult);
                }

                var json = JsonUtility.ToJson(transactionHistory);
                _StorageService.SetString("transactionHistory", json);
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
            Debug.Log($"[BoltSDK] {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[BoltSDK] {message}");
        }

        #endregion
    }
}
