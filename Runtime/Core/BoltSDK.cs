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
        public event Action<PaymentSession> onTransactionComplete;
        public event Action<PaymentSession> onTransactionFailed;
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
                var paymentSessionResult = DeepLinkUtils.ParsePaymentLinkResult(queryParameters);

                if (paymentSessionResult.IsFailed)
                {
                    // Convert paymentSessionResult to PaymentSession for interface compatibility
                    var PaymentSession = new PaymentSession(paymentSessionResult.TransactionId, "");
                    onTransactionFailed?.Invoke(PaymentSession);
                    LogError($"Failed weblink callback for transaction: {paymentSessionResult.ErrorMessage}");
                    return;
                }

                CreateOrUpdateTransaction(paymentSessionResult);
                // Convert paymentSessionResult to PaymentSession for interface compatibility
                var paymentSession = new PaymentSession(paymentSessionResult.TransactionId, "");
                onTransactionComplete?.Invoke(paymentSession);
                LogDebug($"Successful weblink callback for transaction: {paymentSessionResult.TransactionId}");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error during weblink callback: {ex.Message}";
                LogError(errorMessage);
            }
        }

        public List<PaymentSession> GetPaymentSessions()
        {
            try
            {
                var historyData = _StorageService.GetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, "");
                if (string.IsNullOrEmpty(historyData))
                {
                    return new List<PaymentSession>();
                }

                var transactions = JsonUtility.FromJson<List<TransactionResult>>(historyData);
                if (transactions == null)
                {
                    return new List<PaymentSession>();
                }

                // Convert TransactionResult to PaymentSession
                var PaymentSessions = transactions
                    .Where(t => t.Status == TransactionStatus.Pending)
                    .Select(t => new PaymentSession(t.TransactionId, ""))
                    .ToList();

                return PaymentSessions;
            }
            catch (Exception ex)
            {
                LogError($"Failed to load pending payment links: {ex.Message}");
                return new List<PaymentSession>();
            }
        }

        public void RemovePaymentSession(string paymentLinkId)
        {
            try
            {
                var transactionHistory = GetTransactions();
                var transaction = transactionHistory.FirstOrDefault(t => t.TransactionId == paymentLinkId);
                if (transaction != null)
                {
                    transactionHistory.Remove(transaction);
                    var json = JsonUtility.ToJson(transactionHistory);
                    _StorageService.SetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, json);
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

        public List<PaymentSession> GetDevicePaymentSessions()
        {
            try
            {
                var historyData = _StorageService.GetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, "");
                if (string.IsNullOrEmpty(historyData))
                {
                    return new List<PaymentSession>();
                }

                var paymentSessions = JsonUtility.FromJson<List<PaymentSession>>(historyData);
                return paymentSessions ?? new List<PaymentSession>();
            }
            catch (Exception ex)
            {
                LogError($"Failed to load pending transactions: {ex.Message}");
                return new List<TransactionResult>();
            }
        }

        public List<TransactionResult> GetPendingTransactions()
        {
            var paymentSessions = GetDevicePaymentSessions();
            return paymentSessions.Where(t => t.Status == PaymentLinkStatus.Pending).ToList();
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

        private void CreateOrUpdateTransaction(PaymentSession paymentSession)
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
                _StorageService.SetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, json);
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
