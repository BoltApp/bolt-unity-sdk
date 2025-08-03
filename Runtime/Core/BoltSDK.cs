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
        public event Action<PaymentLinkSession> onTransactionComplete;
        public event Action<PaymentLinkSession> onTransactionFailed;
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

        public BoltUser GetBoltUser()
        {
            return GetUserData();
        }

        public BoltUser SetBoltUserData(string email = null, string locale = null, string country = null)
        {
            var user = GetUserData();

            // TODO - provide type validation safety for all fields
            if (email != null)
                user.Email = email;
            if (locale != null)
                user.Locale = locale;
            if (country != null)
                user.Country = country;

            _StorageService.SetObject(BoltPlayerPrefsKeys.USER_DATA, user);
            return user;
        }

        private BoltUser GetUserData()
        {
            var locale = DeviceUtils.GetDeviceLocale();
            var country = DeviceUtils.GetDeviceCountry();
            var deviceId = DeviceUtils.GetDeviceId();

            try
            {
                var user = _StorageService.GetObject<BoltUser>(BoltPlayerPrefsKeys.USER_DATA);

                // Create new user object if user data is not found or is invalid
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

        public void OpenCheckout(string checkoutLink)
        {
            try
            {
                if (string.IsNullOrEmpty(checkoutLink))
                    throw new BoltSDKException("Checkout link cannot be null or empty");

                // Append user info to checkout link
                BoltUser boltUser = GetUserData();
                string checkoutLinkWithParams = UrlUtils.BuildCheckoutLink(checkoutLink, Config, boltUser);

                // Extract payment link id from checkout link
                var queryParameters = UrlUtils.ExtractQueryParameters(checkoutLinkWithParams);
                var paymentLinkId = queryParameters["payment_link_id"];

                // Create a new payment link session or lookup existing one
                PaymentLinkSession paymentLinkSession = GetPaymentLinkSession(paymentLinkId);
                if (paymentLinkSession == null)
                {
                    paymentLinkSession = new PaymentLinkSession(paymentLinkId, checkoutLinkWithParams);
                    SavePaymentLinkSession(paymentLinkSession);
                }
                else
                {
                    paymentLinkSession.UpdateStatus(PaymentLinkStatus.Pending);
                    SavePaymentLinkSession(paymentLinkSession);
                }

                // Invoke callbacks and open checkout link
                LogDebug($"Opening checkout link: {paymentLinkSession.PaymentLinkUrl}");
                onWebLinkOpen?.Invoke();
                Application.OpenURL(paymentLinkSession.PaymentLinkUrl);
            }
            catch (Exception ex)
            {
                LogError($"Failed to open checkout link'{checkoutLink}': {ex.Message}");
                throw;
            }
        }

        public void HandleDeepLinkCallback(string callbackUrl, bool triggerCallbacks = false)
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
                    var data = callbackUrl.Split('=')[1];
                    callbackUrl = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(data));
                }

                // Create a temporary payment link session based on query parameters provided in the callback url
                var queryParameters = UrlUtils.ExtractQueryParameters(callbackUrl);
                var temporalSessionResult = DeepLinkUtils.ParsePaymentLinkSessionResult(queryParameters);
                if (temporalSessionResult == null)
                {
                    LogError($"Failed to parse payment link session result: {callbackUrl}");
                    return;
                }

                // Map temporal session status to existing one on device
                var paymentLinkSession = GetPaymentLinkSession(temporalSessionResult.PaymentLinkId);
                if (paymentLinkSession != null)
                {
                    paymentLinkSession.UpdateStatus(temporalSessionResult.Status);
                    SavePaymentLinkSession(paymentLinkSession);
                }
                else
                {
                    paymentLinkSession = temporalSessionResult;
                    SavePaymentLinkSession(paymentLinkSession);
                }

                // Invoke event based on payment link session status
                if (triggerCallbacks)
                {
                    if (paymentLinkSession.Status == PaymentLinkStatus.Successful)
                    {
                        onTransactionComplete?.Invoke(paymentLinkSession);
                        LogDebug($"Successful weblink callback for transaction: {paymentLinkSession.PaymentLinkId}");
                    }
                    else
                    {
                        onTransactionFailed?.Invoke(paymentLinkSession);
                        LogError($"Failed weblink callback for transaction: {paymentLinkSession.Status}");
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error during weblink callback: {ex.Message}";
                LogError(errorMessage);
            }
        }

        public PaymentLinkSession GetPaymentLinkSession(string paymentLinkId)
        {
            var paymentLinkSessions = GetPaymentLinkSessionHistory();
            return paymentLinkSessions.FirstOrDefault(p => p.PaymentLinkId == paymentLinkId);
        }

        private PaymentLinkSession SavePaymentLinkSession(PaymentLinkSession paymentLinkSession)
        {
            var paymentLinkSessions = GetPaymentLinkSessionHistory();
            var existingPaymentLinkSession = paymentLinkSessions.FirstOrDefault(p => p.PaymentLinkId == paymentLinkSession.PaymentLinkId);
            if (existingPaymentLinkSession != null)
            {
                existingPaymentLinkSession.UpdateStatus(paymentLinkSession.Status);
            }
            else
            {
                paymentLinkSessions.Add(paymentLinkSession);
            }

            var json = JsonUtility.ToJson(paymentLinkSessions);
            _StorageService.SetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, json);
            return paymentLinkSession;
        }

        public PaymentLinkSession ResolvePaymentLinkSession(string paymentLinkId, PaymentLinkStatus status = PaymentLinkStatus.Completed)
        {
            try
            {
                var paymentLinkSessions = GetPaymentLinkSessions();
                var paymentLinkSession = paymentLinkSessions.FirstOrDefault(p => p.PaymentLinkId == paymentLinkId);
                if (paymentLinkSession != null)
                {
                    paymentLinkSessions.Remove(paymentLinkSession);
                    var json = JsonUtility.ToJson(paymentLinkSessions);
                    _StorageService.SetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, json);
                }
                if (status == PaymentLinkStatus.Successful)
                {
                    onTransactionComplete?.Invoke(paymentLinkSession);
                }
                else
                {
                    onTransactionFailed?.Invoke(paymentLinkSession);
                }
                return paymentLinkSession;
            }
            catch (Exception ex)
            {
                LogError($"Failed to resolve payment link session: {ex.Message}");
            }
        }

        public List<PaymentLinkSession> GetPaymentLinkSessionHistory()
        {
            try
            {
                var paymentLinkSessionHistory = _StorageService.GetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, "");
                if (string.IsNullOrEmpty(paymentLinkSessionHistory))
                {
                    return new List<PaymentLinkSession>();
                }

                var paymentLinkSessions = JsonUtility.FromJson<List<PaymentLinkSession>>(historyData);
                return paymentLinkSessions ?? new List<PaymentLinkSession>();
            }
            catch (Exception ex)
            {
                LogError($"Failed to load payment link session history: {ex.Message}");
                return new List<PaymentLinkSession>();
            }
        }

        public List<PaymentLinkSession> GetPendingPaymentLinkSessions()
        {
            var paymentLinkSessions = GetPaymentLinkSessionHistory();
            return paymentLinkSessions.Where(p => p.Status == PaymentLinkStatus.Pending).ToList();
        }

        private void LogDebug(string message)
        {
            Debug.Log($"[BoltSDK] {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[BoltSDK] {message}");
        }
    }
}
