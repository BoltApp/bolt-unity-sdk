using System.Threading.Tasks;
using BoltApp;
using UnityEngine;

namespace BoltApp.Samples
{
    /// <summary>
    /// Basic example showing how to integrate the Bolt SDK
    /// Add this to your game manager or main controller
    /// </summary>
    public class BoltBasicExample : MonoBehaviour
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

                // The app is refocused and we previously tracked checkout open with the onWebLinkOpen callback callback
                // Therefor, the user returned to the app after web checkout but not via deep link
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
                                // Manually mark transaction as cancelled
                                _boltSDK.CancelTransaction(transaction.TransactionId);
                                continue;
                            }

                            if (transactionResult.Status == TransactionStatus.Completed)
                            {
                                // Manually mark transaction as completed
                                _boltSDK.CompleteTransaction(
                                    transactionId = transaction.TransactionId,
                                    isServerVerified = transactionResult.IsServerValidated
                                );
                            }
                            else
                            {
                                // Manually mark transaction as cancelled
                                _boltSDK.CancelTransaction(
                                    transactionId = transaction.TransactionId,
                                    isServerVerified = transactionResult.IsServerValidated
                                );
                            }
                        }
                    }
                }
            }

            // Web checkout is closed, make sure to set any tracking variables
            checkoutIsOpen = false;
        }

        /// <summary>
        /// Mock helper function to verify transaction with backend server
        /// </summary>
        /// <param name="transactionId">The transaction ID to verify</param>
        /// <returns>The transaction result or null if not found</returns>
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