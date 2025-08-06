using System.Threading.Tasks;
using BoltApp;
using UnityEngine;

namespace BoltApp.Samples
{
    /// <summary>
    /// Optional: Deep Link example using web callbacks
    /// Use this as reference for your own implementation.
    /// </summary>
    public class BoltDeepLinkExample : MonoBehaviour
    {
        private BoltSDK boltSDK;

        // It is best practice to store deep links in a queue to handle multiple links back to the app
        private Queue<string> pendingDeepLinks = new Queue<string>();

        void Start()
        {
            var boltConfig = new BoltConfig(
                "com.yourgameid.test",
                "YourAppNameForDeepLinks://",
                BoltConfig.Environment.Development);
            boltSDK = new BoltSDK(boltConfig);
            boltSDK.onTransactionComplete += OnTransactionComplete;
            boltSDK.onTransactionFailed += OnTransactionFailed;
            boltSDK.onWebLinkOpen += onWebLinkOpen;

            // Process any deep links that were received before the SDK was initialized
            OnResume();
        }

        void OnApplicationFocus(bool hasFocus)
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
                pendingDeepLinks.Enqueue(Application.deepLink);
                Application.deepLink = null;
            }

            // Check for transactions or deep links on app resume
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                pendingDeepLinks.Enqueue(Application.absoluteURL);
                Application.absoluteURL = null;
            }

            ProcessPendingDeepLinks();
        }

        async void ProcessPendingDeepLinks()
        {
            while (pendingDeepLinks.Count > 0)
            {
                var deepLink = pendingDeepLinks.Dequeue();
                PaymentLinkSession paymentLinkSession = boltSDK.HandleDeepLinkCallback(deepLink);
                if (paymentLinkSession != null)
                {
                    // Optional: Call your backend to verify the payment link status
                    var paymentLinkResult = await VerifyPaymentLinkSuccess(paymentLinkSession.PaymentLinkId);

                    // Resolve the payment link session with the result
                    // Note: This SDK call will trigger the onTransactionComplete or onTransactionFailed callbacks
                    boltSDK.ResolvePaymentLinkSession(paymentLinkSession.PaymentLinkId, paymentLinkResult);
                }
            }
        }

        /// <summary>
        /// Mock helper function to verify transaction with backend server
        /// </summary>
        /// <param name="paymentLinkId">The payment link ID to verify</param>
        /// <returns>The payment link status</returns>
        private PaymentLinkStatus VerifyPaymentLinkSuccess(string paymentLinkId)
        {
            // TODO: Use your http client to call backend to verify the transactionID was successful
            // In that server call you can check for a webhook or perform a GET on the transactionID

            // Example response
            return PaymentLinkStatus.Successful;
        }

        /// <summary>
        /// When transaction is complete, this callback will be called.
        /// Show a success screen and fire any analytic events here.
        /// </summary>
        private void OnTransactionComplete(TransactionResult result)
        {
            Debug.Log("Transaction success: " + result.TransactionId);
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
            // Consider firing analytic event here or dimming the screen to indicate that the checkout is open
            Debug.Log("Checkout open.");
        }
    }
}