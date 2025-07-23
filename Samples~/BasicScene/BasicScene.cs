using BoltSDK;
using UnityEngine;
using UnityEngine.UI;

namespace BoltSDK.Samples
{
    /// <summary>
    /// Simple basic scene that uses the BoltBasicExample as a GameObject
    /// with a simple UI for opening web links
    /// </summary>
    public class BasicScene : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button openWebLinkButton;
        [SerializeField] private Button openCheckoutButton;
        [SerializeField] private Text statusText;

        [Header("Web Links")]
        [SerializeField] private string webLink = "https://www.bolt.com";
        [SerializeField] private string checkoutLink = "https://checkout.bolt.com/test";

        private BoltManager _boltManager;

        void Start()
        {
            // Find or create the BoltManager (BoltBasicExample)
            _boltManager = FindObjectOfType<BoltManager>();
            if (_boltManager == null)
            {
                GameObject boltManagerObj = new GameObject("BoltManager");
                _boltManager = boltManagerObj.AddComponent<BoltManager>();
            }

            SetupUI();
            UpdateStatus("Scene ready - Bolt SDK initialized");
        }

        /// <summary>
        /// Set up the UI buttons
        /// </summary>
        private void SetupUI()
        {
            if (openWebLinkButton != null)
            {
                openWebLinkButton.onClick.AddListener(OpenWebLink);
            }

            if (openCheckoutButton != null)
            {
                openCheckoutButton.onClick.AddListener(OpenCheckout);
            }
        }

        /// <summary>
        /// Open a web link
        /// </summary>
        public void OpenWebLink()
        {
            UpdateStatus($"Opening web link: {webLink}");
            Application.OpenURL(webLink);
        }

        /// <summary>
        /// Open a checkout link
        /// </summary>
        public void OpenCheckout()
        {
            UpdateStatus($"Opening checkout: {checkoutLink}");
            Application.OpenURL(checkoutLink);
        }

        /// <summary>
        /// Update the status text
        /// </summary>
        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log(message);
        }
    }
}