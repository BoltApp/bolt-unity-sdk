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
        public event Action<PendingPaymentLink> onTransactionComplete;
        public event Action<PendingPaymentLink> onTransactionFailed;
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

        public BoltUser SetBoltUserData(string email = null, string locale = null, string country = null)
        {
            var user = GetUserData();

            // TODO - provide type validation safety
            if (email != null)
                user.Email = email;
            if (locale != null)
                user.Locale = locale;
            if (country != null)
                user.Country = country;

            _StorageService.SetObject(BoltPlayerPrefsKeys.USER_DATA, user);
            return user;
        }

        public BoltUser GetBoltUser()
        {
            return GetUserData();
        }

        public void OpenCheckout(string checkoutLink)
        {
            try
            {
                if (string.IsNullOrEmpty(checkoutLink))
                    throw new BoltSDKException("Checkout link cannot be null or empty");

                BoltUser boltUser = GetUserData();
                string checkoutLinkWithParams = UrlUtils.BuildCheckoutLink(checkoutLink, Config, boltUser);

                LogDebug($"Opening checkout link: {checkoutLinkWithParams}");
                onWebLinkOpen?.Invoke();
                Application.OpenURL(checkoutLinkWithParams);
            }
            catch (Exception ex)
            {
                LogError($"Failed to open checkout link'{checkoutLinkWithParams}': {ex.Message}");
                throw;
            }
        }

        public void HandleDeepLinkCallback(string callbackUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(callbackUrl))
                {
                    var errorMessage = "Failed to parse transaction. 'callbackUrl' cannot be null or empty";
                    LogError(errorMessage);
                    return;
                }

                // Check if there is a data field in the callback url, if so then base64 decode it
                if (callbackUrl.Contains("data="))
                {
                    var data = callbackUrl.Split('data=')[1];
                    callbackUrl = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(data));
                }

                var queryParameters = UrlUtils.ExtractQueryParameters(callbackUrl);
                var paymentLinkResult = DeepLinkUtils.ParsePaymentLinkResult(queryParameters);

                if (transactionResult.IsFailed)
                {
                    // Convert TransactionResult to PendingPaymentLink for interface compatibility
                    var pendingPaymentLink = new PendingPaymentLink(transactionResult.TransactionId, "");
                    onTransactionFailed?.Invoke(pendingPaymentLink);
                    LogError($"Failed weblink callback for transaction: {transactionResult.ErrorMessage}");
                    return;
                }

                CreateOrUpdateTransaction(transactionResult);
                // Convert TransactionResult to PendingPaymentLink for interface compatibility
                var paymentLink = new PendingPaymentLink(transactionResult.TransactionId, "");
                onTransactionComplete?.Invoke(paymentLink);
                LogDebug($"Successful weblink callback for transaction: {transactionResult.TransactionId}");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error during weblink callback: {ex.Message}";
                LogError(errorMessage);
            }
        }

        public List<PendingPaymentLink> GetPendingPaymentLinks()
        {
            try
            {
                var historyData = _StorageService.GetString(BoltPlayerPrefsKeys.TRANSACTION_HISTORY, "");
                if (string.IsNullOrEmpty(historyData))
                {
                    return new List<PendingPaymentLink>();
                }

                var transactions = JsonUtility.FromJson<List<TransactionResult>>(historyData);
                if (transactions == null)
                {
                    return new List<PendingPaymentLink>();
                }

                // Convert TransactionResult to PendingPaymentLink
                var pendingPaymentLinks = transactions
                    .Where(t => t.Status == TransactionStatus.Pending)
                    .Select(t => new PendingPaymentLink(t.TransactionId, ""))
                    .ToList();

                return pendingPaymentLinks;
            }
            catch (Exception ex)
            {
                LogError($"Failed to load pending payment links: {ex.Message}");
                return new List<PendingPaymentLink>();
            }
        }

        public void RemovePendingPaymentLink(string paymentLinkId)
        {
            try
            {
                var transactionHistory = GetTransactions();
                var transaction = transactionHistory.FirstOrDefault(t => t.TransactionId == paymentLinkId);
                if (transaction != null)
                {
                    transactionHistory.Remove(transaction);
                    var json = JsonUtility.ToJson(transactionHistory);
                    _StorageService.SetString(BoltPlayerPrefsKeys.TRANSACTION_HISTORY, json);
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to remove pending payment link: {ex.Message}");
            }
        }

        #region Private Methods

        private BoltUser GetUserData()
        {
            var locale = DeviceUtils.GetDeviceLocale();
            var country = DeviceUtils.GetDeviceCountry();
            var deviceId = DeviceUtils.GetDeviceId();

            try
            {
                var user = _StorageService.GetObject<BoltUser>(BoltPlayerPrefsKeys.USER_DATA);
                if (user == null || user.DeviceId != deviceId || user.Locale != locale || user.Country != country)
                {
                    var email = user?.Email ?? "";
                    user = new BoltUser(email, locale, country, deviceId);
                }
                _StorageService.SetObject(BoltPlayerPrefsKeys.USER_DATA, user);
                return user;
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize user data: {ex.Message}");
                var newUser = new BoltUser("", locale, country, deviceId);
                _StorageService.SetObject(BoltPlayerPrefsKeys.USER_DATA, newUser);
                return newUser;
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
                var historyData = _StorageService.GetString(BoltPlayerPrefsKeys.TRANSACTION_HISTORY, "");
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
                _StorageService.SetString(BoltPlayerPrefsKeys.TRANSACTION_HISTORY, json);
            }
            catch (Exception ex)
            {
                LogError($"Failed to save pending transactions: {ex.Message}");
            }
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
