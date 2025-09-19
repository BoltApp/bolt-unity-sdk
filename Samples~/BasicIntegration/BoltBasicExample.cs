using System.Threading.Tasks;
using UnityEngine;
using BoltApp;

namespace BoltApp.Samples
{
    /// <summary>
    /// Basic example showing how to integrate the Bolt SDK and handle payment links
    /// Use this as reference for your own implementation.
    /// </summary>
    public class BoltBasicExample : MonoBehaviour
    {
        private BoltSDK boltSDK;
        private bool checkoutIsOpen = false;

        void Start()
        {
            var boltConfig = new BoltConfig(
                "com.myapp.test",
                "example.publishable.key",
                BoltConfig.Environment.Development);

            // Setup SDK
            var boltSDK = new BoltSDK(boltConfig);

            // Setup callbacks, handle flows appropriately
            boltSDK.onTransactionComplete += OnTransactionComplete;
            boltSDK.onTransactionFailed += OnTransactionFailed;
            boltSDK.onWebLinkOpen += onWebLinkOpen;

            // The SDK handles user data automatically. Use as needed in your APIs and Analytics.
            var user = boltSDK.GetBoltUser();
            Debug.Log("User: " + user.ToString());

            // Open A Checkout Link, typically assigned to a button click in your UI
            // Note: SDK automatically stores a pending payment link in player prefs, used in 'VerifyRecentCheckouts()'
            var checkoutLinkFetchedFromYourBackend = "https://knights-of-valor-bolt.c-staging.bolt.com/c?u=Fv8ZMmDmRb86C4XRiB92x2&publishable_key=_Kq5XZXqaLiS.3TOhnz9Wmacb.9c59b297d066e94294895dd8617ad5d9d8ffc530fe1d36f8ed6d624a4f7855ae";
            boltSDK.OpenCheckout(checkoutLinkFetchedFromYourBackend);

            // Note: On app load you need to check for recent checkouts.
            // Look at 'VerifyRecentCheckouts()' in this file for an example.
        }

        /// <summary>
        /// If user manually opens the app again, check the status of the latest transactions.
        /// </summary>
        async Task OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                // The app is refocused and we previously tracked checkout open with the onWebLinkOpen callback
                // Therefore, the user returned to the app after web checkout but not via deep link
                if (checkoutIsOpen)
                {
                    VerifyRecentTransactions();
                }
            }

            // Web checkout is closed, make sure to update any of your UI variables
            checkoutIsOpen = false;
        }

        /// <summary>
        /// Verify the status of the latest checkouts with the backend server
        /// </summary>
        private async void VerifyRecentCheckouts()
        {
            // Check status of latest transaction with backend server
            var pendingPaymentLinkSessions = boltSDK.GetPendingPaymentLinkSessions();
            if (pendingPaymentLinkSessions.Count > 0)
            {
                foreach (var paymentLinkSession in pendingPaymentLinkSessions)
                {
                    // Call your backend to verify the payment link status
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
        private async PaymentLinkStatus VerifyPaymentLinkSuccess(string paymentLinkId)
        {
            // TODO: Use your http client to call backend to verify the payment link status
            // In that server call you can check for a webhook or perform a GET on the payment link ID

            // Example response
            return PaymentLinkStatus.Successful;
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
            Debug.Log("Checkout open for user: " + boltSDK.GetBoltUser().ToString());
            checkoutIsOpen = true;
        }
    }
}