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

                // Create a new payment session or lookup existing one
                PaymentSession paymentSession = GetPaymentSession(paymentLinkId);
                if (paymentSession == null)
                {
                    paymentSession = new PaymentSession(paymentLinkId, checkoutLinkWithParams);
                    SavePaymentSession(paymentSession);
                }
                else
                {
                    paymentSession.UpdateStatus(PaymentLinkStatus.Pending);
                    SavePaymentSession(paymentSession);
                }

                // Invoke callbacks and open checkout link
                LogDebug($"Opening checkout link: {paymentSession.PaymentLinkUrl}");
                onWebLinkOpen?.Invoke();
                Application.OpenURL(paymentSession.PaymentLinkUrl);
            }
            catch (Exception ex)
            {
                LogError($"Failed to open checkout link'{checkoutLink}': {ex.Message}");
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
                    var data = callbackUrl.Split('=')[1];
                    callbackUrl = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(data));
                }

                // Create a temporary payment session based on query parameters provided in the callback url
                var queryParameters = UrlUtils.ExtractQueryParameters(callbackUrl);
                var temporalSessionResult = DeepLinkUtils.ParsePaymentLinkResult(queryParameters);

                // Map temporal session status to existing one on device
                var paymentSession = GetPaymentSession(temporalSessionResult.PaymentLinkId);
                if (paymentSession != null)
                {
                    paymentSession.UpdateStatus(temporalSessionResult.Status);
                    SavePaymentSession(paymentSession);
                }
                else
                {
                    paymentSession = temporalSessionResult;
                    SavePaymentSession(paymentSession);
                }

                // Invoke event based on payment session status
                if (paymentSession.Status == PaymentLinkStatus.Successful)
                {
                    onTransactionComplete?.Invoke(paymentSession);
                    LogDebug($"Successful weblink callback for transaction: {paymentSession.PaymentLinkId}");
                }
                else
                {
                    onTransactionFailed?.Invoke(paymentSession);
                    LogError($"Failed weblink callback for transaction: {paymentSession.Status}");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error during weblink callback: {ex.Message}";
                LogError(errorMessage);
            }
        }

        public PaymentSession GetPaymentSession(string paymentLinkId)
        {
            var paymentSessions = GetPaymentSessionHistory();
            return paymentSessions.FirstOrDefault(p => p.PaymentLinkId == paymentLinkId);
        }

        private PaymentSession SavePaymentSession(PaymentSession paymentSession)
        {
            var paymentSessions = GetPaymentSessionHistory();
            var existingPaymentSession = paymentSessions.FirstOrDefault(p => p.PaymentLinkId == paymentSession.PaymentLinkId);
            if (existingPaymentSession != null)
            {
                existingPaymentSession.UpdateStatus(paymentSession.Status);
            }
            else
            {
                paymentSessions.Add(paymentSession);
            }

            var json = JsonUtility.ToJson(paymentSessions);
            _StorageService.SetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, json);
            return paymentSession;
        }

        public void ResolvePaymentSession(string paymentLinkId, PaymentLinkStatus status = PaymentLinkStatus.Completed)
        {
            try
            {
                var paymentSessions = GetPaymentSessions();
                var paymentSession = paymentSessions.FirstOrDefault(p => p.PaymentLinkId == paymentLinkId);
                if (paymentSession != null)
                {
                    paymentSessions.Remove(paymentSession);
                    var json = JsonUtility.ToJson(paymentSessions);
                    _StorageService.SetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, json);
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to resolve payment session: {ex.Message}");
            }
        }

        public List<PaymentSession> GetPaymentSessionHistory()
        {
            try
            {
                var paymentSessionHistory = _StorageService.GetString(BoltPlayerPrefsKeys.PAYMENT_SESSION_HISTORY, "");
                if (string.IsNullOrEmpty(paymentSessionHistory))
                {
                    return new List<PaymentSession>();
                }

                var paymentSessions = JsonUtility.FromJson<List<PaymentSession>>(historyData);
                return paymentSessions ?? new List<PaymentSession>();
            }
            catch (Exception ex)
            {
                LogError($"Failed to load payment session history: {ex.Message}");
                return new List<PaymentSession>();
            }
        }

        public List<PaymentSession> GetPendingPaymentSessions()
        {
            var paymentSessions = GetPaymentSessionHistory();
            return paymentSessions.Where(p => p.Status == PaymentLinkStatus.Pending).ToList();
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
