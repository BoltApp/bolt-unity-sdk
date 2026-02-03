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
        public event Action onAdOpened;
        public event Action onAdCompleted;
        public BoltConfig Config { get; private set; }
        private IStorageService _StorageService;
        private IAdWebViewService _AdWebViewService;

        // In-memory dictionary for pending payment link sessions only
        private Dictionary<string, PaymentLinkSession> _pendingPaymentLinkSessions;
        
        private AdSession _adSession;

        public BoltSDK()
        {
            Config = new BoltConfig();
            _StorageService = new PlayerPrefsStorageService();
            LoadPendingPaymentLinkSessionsFromStorage();
        }
        public BoltSDK(BoltConfig config)
        {
            Config = config;
            _StorageService = new PlayerPrefsStorageService();
            LoadPendingPaymentLinkSessionsFromStorage();
        }

        public BoltSDK(IAdWebViewService adWebViewService)
        {
            Config = new BoltConfig();
            _StorageService = new PlayerPrefsStorageService();
            _AdWebViewService = adWebViewService;
            LoadPendingPaymentLinkSessionsFromStorage();
        }

        public BoltSDK(BoltConfig config, IAdWebViewService adWebViewService)
        {
            Config = config;
            _StorageService = new PlayerPrefsStorageService();
            _AdWebViewService = adWebViewService;
            LoadPendingPaymentLinkSessionsFromStorage();
        }

        private void LoadPendingPaymentLinkSessionsFromStorage()
        {
            try
            {
                var allSessions = _StorageService.GetDictionary<string, PaymentLinkSession>(
                    BoltPlayerPrefsKeys.PENDING_PAYMENT_SESSIONS,
                    new Dictionary<string, PaymentLinkSession>());

                // Only keep pending links, some defensive programming for older versions of the SDK
                _pendingPaymentLinkSessions = allSessions
                    .Where(kvp => kvp.Value.Status == PaymentLinkStatus.Pending)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                LogDebug($"Loaded {_pendingPaymentLinkSessions.Count} pending payment link sessions from storage");
            }
            catch (Exception ex)
            {
                LogError($"Failed to load pending payment link sessions from storage: {ex.Message}");
                _pendingPaymentLinkSessions = new Dictionary<string, PaymentLinkSession>();
            }
        }

        private void SavePendingPaymentLinkSessionsToStorage()
        {
            try
            {
                _StorageService.SetDictionary(BoltPlayerPrefsKeys.PENDING_PAYMENT_SESSIONS, _pendingPaymentLinkSessions);
                _StorageService.Save();
                LogDebug($"Saved {_pendingPaymentLinkSessions.Count} pending payment link sessions to storage");
            }
            catch (Exception ex)
            {
                LogError($"Failed to save pending payment link sessions to storage: {ex.Message}");
            }
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

            SaveUserData(user);
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
                    SaveUserData(user);
                }

                return user;
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize user data: {ex.Message}");
                var newUser = new BoltUser("", locale, country, deviceId);
                SaveUserData(newUser);
                return newUser;
            }
        }

        private void SaveUserData(BoltUser user)
        {
            _StorageService.SetObject(BoltPlayerPrefsKeys.USER_DATA, user);
            _StorageService.Save();
        }

        public void OpenCheckout(string checkoutLink)
        {
            try
            {
                if (string.IsNullOrEmpty(checkoutLink))
                {
                    LogError("Checkout link cannot be null or empty");
                    return;
                }

                // Append user info to checkout link
                BoltUser boltUser = GetUserData();
                string checkoutLinkWithParams = UrlUtils.BuildCheckoutLink(checkoutLink, Config, boltUser);

                // Extract payment link id from checkout link
                var queryParameters = UrlUtils.ExtractQueryParameters(checkoutLinkWithParams);
                var paymentLinkId = queryParameters.ContainsKey("payment_link_id") ? queryParameters["payment_link_id"] : "";
                if (string.IsNullOrEmpty(paymentLinkId))
                {
                    LogError("Failed to extract payment_link_id from checkout link");
                    return;
                }

                // Create a new payment link session or lookup existing one
                PaymentLinkSession paymentLinkSession = GetPaymentLinkSession(paymentLinkId);
                if (paymentLinkSession == null || !paymentLinkSession.IsValid())
                {
                    paymentLinkSession = new PaymentLinkSession(paymentLinkId, checkoutLinkWithParams);
                    AddPendingPaymentLinkSession(paymentLinkSession);
                }
                else
                {
                    paymentLinkSession.UpdateStatus(PaymentLinkStatus.Pending);
                    AddPendingPaymentLinkSession(paymentLinkSession);
                }

                // Save to storage before opening checkout
                SavePendingPaymentLinkSessionsToStorage();

                // Invoke callbacks and open checkout link
                LogDebug($"Opening checkout link: {paymentLinkSession.PaymentLinkUrl}");
                onWebLinkOpen?.Invoke();
                Application.OpenURL(paymentLinkSession.PaymentLinkUrl);
            }
            catch (Exception ex)
            {
                LogError($"Failed to open checkout link '{checkoutLink}': {ex.Message}");
            }
        }

        public PaymentLinkSession HandleDeepLinkCallback(string callbackUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(callbackUrl))
                {
                    LogError("Failed to parse transaction. 'callbackUrl' cannot be null or empty");
                    return null;
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
                    return null;
                }

                // Map temporal session status to existing one on device
                var paymentLinkSession = GetPaymentLinkSession(temporalSessionResult.PaymentLinkId);
                if (paymentLinkSession != null)
                {
                    paymentLinkSession.UpdateStatus(temporalSessionResult.Status);
                }
                else
                {
                    paymentLinkSession = temporalSessionResult;
                }

                // Save the session if it's pending, or remove it if it's resolved
                if (paymentLinkSession.Status == PaymentLinkStatus.Pending)
                {
                    AddPendingPaymentLinkSession(paymentLinkSession);
                    SavePendingPaymentLinkSessionsToStorage();
                }
                else
                {
                    // If we have a resolved session that wasn't in our pending list, 
                    // we don't need to store it since we only care about pending
                    LogDebug($"Received resolved payment link session: {paymentLinkSession.PaymentLinkId} with status: {paymentLinkSession.Status}");
                }

                return paymentLinkSession;
            }
            catch (Exception ex)
            {
                LogError($"Error during weblink callback: {ex.Message}");
                return null;
            }
        }

        public PaymentLinkSession GetPaymentLinkSession(string paymentLinkId)
        {
            _pendingPaymentLinkSessions.TryGetValue(paymentLinkId, out var session);
            return session;
        }

        private PaymentLinkSession AddPendingPaymentLinkSession(PaymentLinkSession paymentLinkSession)
        {
            if (!paymentLinkSession.IsValid())
            {
                LogError($"Invalid payment link session: {paymentLinkSession.PaymentLinkId}");
                return null;
            }

            // Only save if status is pending
            if (paymentLinkSession.Status == PaymentLinkStatus.Pending)
            {
                _pendingPaymentLinkSessions[paymentLinkSession.PaymentLinkId] = paymentLinkSession;
            }

            return paymentLinkSession;
        }

        public PaymentLinkSession ResolvePaymentLinkSession(string paymentLinkId, PaymentLinkStatus status = PaymentLinkStatus.Successful, bool autoSave = true)
        {
            try
            {
                var paymentLinkSession = GetPaymentLinkSession(paymentLinkId);
                if (paymentLinkSession != null)
                {
                    paymentLinkSession.UpdateStatus(status);

                    // Remove from pending sessions when resolved (memory efficient)
                    if (status != PaymentLinkStatus.Pending)
                    {
                        _pendingPaymentLinkSessions.Remove(paymentLinkId);

                        if (autoSave)
                        {
                            SavePendingPaymentLinkSessionsToStorage();
                        }
                    }
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
                return null;
            }
        }

        public Dictionary<string, PaymentLinkSession> GetPendingPaymentLinkSessions()
        {
            return new Dictionary<string, PaymentLinkSession>(_pendingPaymentLinkSessions);
        }

        public bool HasPendingPaymentLinkSessions()
        {
            return _pendingPaymentLinkSessions.Count > 0;
        }

        private void LogDebug(string message)
        {
            Debug.Log($"[BoltSDK] {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[BoltSDK] {message}");
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning($"[BoltSDK] {message}");
        }

        public void ManualSave()
        {
            SaveUserData(GetUserData());
            SavePendingPaymentLinkSessionsToStorage();
        }

        public AdSession PreloadAd()
        {
            try
            {
                if (_AdWebViewService == null)
                {
                    LogError("IAdWebViewService is not set. Please provide an IAdWebViewService implementation when creating BoltSDK.");
                    return null;
                }

                var adLink = Config.GetAdLink();
                LogDebug($"Preloading ad link: {adLink}");

                _AdWebViewService.Preload(adLink);
                _AdWebViewService.SetOnClaimCallback(HandleAdClaim);

                var adSession = new AdSession(adLink, (link) =>
                {
                    LogDebug($"Ad completed");
                    onAdCompleted?.Invoke();
                });

                LogDebug($"Ad preloaded");
                _adSession = adSession;
                return adSession;
            }
            catch (Exception ex)
            {
                LogError($"Failed to preload advertisement: {ex.Message}");
                throw;
            }
        }

        private void ShowAdWebView(AdSession adSession)
        {
            if (adSession == null || !adSession.IsValid())
            {
                LogError("Cannot show webview for invalid ad session");
                return;
            }

            if (_AdWebViewService == null)
            {
                LogError("IAdWebViewService is not set");
                adSession.UpdateStatus(AdStatus.Failed, "WebView service not available");
                return;
            }

            try
            {
                _AdWebViewService.Show();
                onAdOpened?.Invoke();
            }
            catch (Exception ex)
            {
                LogError($"Failed to show webview: {ex.Message}");
                adSession.UpdateStatus(AdStatus.Failed, ex.Message);
                _adSession = null;
            }
        }

        private void HandleAdClaim()
        {
            if (_adSession == null || _adSession.Status != AdStatus.Showing)
            {
                LogWarning("ad claim received without a shown ad session");
                return;
            }

            _adSession.UpdateStatus(AdStatus.Completed);
            _adSession.FireOnCompleted();
            _AdWebViewService?.Cleanup();
            _adSession = null;

            try
            {
                PreloadAd();
            }
            catch (Exception ex)
            {
                LogError($"Failed to preload next ad: {ex.Message}");
            }
        }

        public AdSession ShowAd()
        {
            if (_adSession == null || _adSession.Status != AdStatus.Preloaded)
            {
                var failedSession = new AdSession();
                failedSession.UpdateStatus(AdStatus.Failed, "No preloaded ad available");
                return failedSession;
            }

            _adSession.UpdateStatus(AdStatus.Showing);
            ShowAdWebView(_adSession);
            return _adSession;
        }
    }
}