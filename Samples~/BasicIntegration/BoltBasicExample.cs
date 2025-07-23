using System.Threading.Tasks;
using BoltApp;
using UnityEngine;

namespace BoltApp.Samples
{
    /// <summary>
    /// Basic example showing how to integrate the Bolt SDK
    /// Add this to your game manager or main controller
    /// </summary>
    public class BoltManager : MonoBehaviour
    {
        private BoltSDK _boltSDK;
        private bool checkoutIsOpen = false;

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

        /// <summary>
        /// If user manually opens the app again, check the status of the latest transactions.
        /// </summary>
        async Task OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Debug.Log("App focused again");

                if (checkoutIsOpen)
                {
                    // Check status of latest transaction with backend server
                    var pendingTransactions = _boltSDK.GetPendingTransactions();
                    if (pendingTransactions.Count > 0)
                    {
                        foreach (var transaction in pendingTransactions)
                        {
                            var transactionResult = await ServerVerifyTransaction(transaction.TransactionId);
                            if (transactionResult == null)
                            {
                                _boltSDK.CancelTransaction(transaction.TransactionId, false);
                                continue;
                            }

                            if (transactionResult.Status == TransactionStatus.Completed)
                            {
                                _boltSDK.CompleteTransaction(transaction.TransactionId, true);
                            }
                            else
                            {
                                _boltSDK.CancelTransaction(transaction.TransactionId, true);
                            }
                        }
                    }
                }
            }

            // Web checkout is closed, make sure to set any tracking variables
            checkoutIsOpen = false;
        }

        private TransactionResult ServerVerifyTransaction(string transactionId)
        {
            // TODO - Use your http client to call backend to verify
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
        /// When redirected back from web, handle deep link here.
        /// </summary>
        private void HandleDeepLink(string deepLink)
        {
            _boltSDK.HandleDeepLinkCallback(deepLink);
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