using BoltSDK;
using UnityEngine;

namespace BoltSDK.Samples
{
    /// <summary>
    /// Basic example showing how to integrate the Bolt SDK
    /// Add this to your game manager or main controller
    /// </summary>
    public class BoltManager : MonoBehaviour
    {
        private BoltSDK _boltSDK;

        void Start()
        {
            // Will init using values from the BoltSDKConfig asset in the Resources folder.
            _boltSDK = new BoltSDK();

            // Subscribe to events
            _boltSDK.onTransactionComplete += OnTransactionComplete;
            _boltSDK.onTransactionFailed += OnTransactionFailed;

            // Optional: Subscribe to web link open event for analytics.
            _boltSDK.onWebLinkOpen += onWebLinkOpen;
        }

        void OnDestroy()
        {
            _boltSDK?.Dispose();
        }

        /// <summary>
        /// When redirected back from web, handle deep link here.
        /// </summary>
        private void HandleDeepLink(string deepLink)
        {
            _boltSDK.HandleWeblinkCallback(deepLink);
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
        }
    }
}