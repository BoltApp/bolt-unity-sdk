using System.Threading.Tasks;
using BoltApp;
using UnityEngine;

namespace BoltApp.Samples
{
    /// <summary>
    /// Basic example showing how to integrate the Bolt SDK
    /// Use this as reference for your own implementation.
    /// </summary>
    public class BoltBasicExample : MonoBehaviour
    {
        private BoltSDK _boltSDK;
        private bool checkoutIsOpen = false;

        void Start()
        {
            var boltConfig = new BoltConfig(
                "com.myapp.test",
                "example.publishable.key",
                BoltConfig.Environment.Development);

            // Setup SDK
            _boltSDK = new BoltSDK(boltConfig);

            // Setup callbacks, handle flows appropriately
            _boltSDK.onTransactionComplete += OnTransactionComplete;
            _boltSDK.onTransactionFailed += OnTransactionFailed;
            _boltSDK.onWebLinkOpen += onWebLinkOpen;

            // Fetch User data, use as needed in your APIs and Analytics. Bolt SDK manages user data for you.
            var user = _boltSDK.GetBoltUser();
            Debug.Log("User: " + user.ToString());

            // Open A Checkout Link, typically assigned to a button in your UI
            // Note: SDK automatically stores a pending payment link, used in next step.
            var checkoutLinkFetchedFromYourBackend = "https://knights-of-valor-bolt.c-staging.bolt.com/c?u=Fv8ZMmDmRb86C4XRiB92x2&publishable_key=_Kq5XZXqaLiS.3TOhnz9Wmacb.9c59b297d066e94294895dd8617ad5d9d8ffc530fe1d36f8ed6d624a4f7855ae";
            _boltSDK.OpenCheckout(checkoutLinkFetchedFromYourBackend);

            // Look at VerifyRecentCheckouts() for how to handle app load and verify checkout results
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
        private void VerifyRecentCheckouts()
        {
            // Check status of latest transaction with backend server
            var pendingPaymentLinkSessions = _boltSDK.GetPendingPaymentLinkSessions();
            if (pendingPaymentLinkSessions.Count > 0)
            {
                foreach (var paymentLinkSession in pendingPaymentLinkSessions)
                {
                    var paymentLinkResult = await ServerVerifyPaymentLink(paymentLinkSession.PaymentLinkId);
                    _boltSDK.ResolvePaymentLinkSession(paymentLinkSession.PaymentLinkId, paymentLinkResult.Status);
                }
            }
        }

        /// <summary>
        /// Mock helper function to verify transaction with backend server
        /// </summary>
        /// <param name="transactionId">The transaction ID to verify</param>
        /// <returns>The transaction result or null if not found</returns>
        private TransactionResult ServerVerifyPaymentLink(string transactionId)
        {
            // TODO - Use your http client to call backend to verify and get the following object back:
            var mockResult = new TransactionResult(
                TransactionId = transactionId,
                Status = TransactionStatus.Completed,
                IsServerValidated = true,
                Amount = 100,
                Currency = "USD",
                ProductId = "example_product_id",
                UserEmail = "example@example.com",
                Timestamp = DateTime.UtcNow
            );
            return mockResult;
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
            Debug.Log("Checkout open for user: " + _boltSDK.GetBoltUser().ToString());
            checkoutIsOpen = true;
        }
    }
}