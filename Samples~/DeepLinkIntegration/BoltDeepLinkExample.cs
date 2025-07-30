using System.Threading.Tasks;
using BoltApp;
using UnityEngine;

namespace BoltApp.Samples
{
    /// <summary>
    /// Basic example showing how to integrate the Bolt SDK
    /// Add this to your game manager or main controller
    /// </summary>
    public class BoltDeepLinkExample : MonoBehaviour
    {
        private BoltSDK _boltSDK;

        // It is best practice to store deep links in a queue to handle multiple deep links
        private Queue<string> _pendingDeepLinks = new Queue<string>();


        void Start()
        {
            var boltConfig = new BoltConfig(
                "com.myapp.test",
                "MyAppNameForDeepLinks",
                BoltConfig.Environment.Development);
            _boltSDK = new BoltSDK(boltConfig);
            _boltSDK.onTransactionComplete += OnTransactionComplete;
            _boltSDK.onTransactionFailed += OnTransactionFailed;
            _boltSDK.onWebLinkOpen += onWebLinkOpen;
        }

        void Awake()
        {
            // Check for transactions or deep links on app on load
            OnResume();

        }

        async Task OnApplicationFocus(bool hasFocus)
        {
            // Check for transactions or deep links on app resume
            if (hasFocus)
            {
                OnResume();
            }
        }

        void OnResume()
        {
            // Check Unity's built-in deep link properties
            if (!string.IsNullOrEmpty(Application.deepLink))
            {
                _pendingDeepLinks.Enqueue(Application.deepLink);
                Application.deepLink = null;
            }

            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                _pendingDeepLinks.Enqueue(Application.absoluteURL);
                Application.absoluteURL = null;
            }

            ProcessPendingDeepLinks();
        }

        void ProcessPendingDeepLinks()
        {
            while (_pendingDeepLinks.Count > 0)
            {
                var deepLink = _pendingDeepLinks.Dequeue();
                _boltSDK.HandleDeepLinkCallback(deepLink);
            }
        }

        /// <summary>
        /// When transaction is complete, this callback will be called.
        /// Show a success screen and fire any analytic events here.
        /// </summary>
        private void OnTransactionComplete(TransactionResult result)
        {
            Debug.Log("Transaction complete: " + result.TransactionId);
        }

        /// <summary>
        /// When transaction is failed, this callback will be called.
        /// Show a failure screen and fire any analytic events here.
        /// </summary>
        private void OnTransactionFailed(TransactionResult result)
        {
            Debug.Log("Transaction failed: " + result.TransactionId);
        }

        /// <summary>
        /// Optional: When web link is open, this callback will be called.
        /// Fire any analytic events here.
        /// </summary>
        private void onWebLinkOpen()
        {
            // Consider firing analytic event here.
            Debug.Log("Checkout open.");
            checkoutIsOpen = true;
        }
    }
}