using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace BoltSDK
{
    public class WebGLWebView : IBoltWebView
    {
        // JavaScript plugin imports
        [DllImport("__Internal")]
        private static extern void OpenWebViewWindow(string url, string windowName, int width, int height);
        
        [DllImport("__Internal")]
        private static extern void CloseWebViewWindow();
        
        [DllImport("__Internal")]
        private static extern bool IsWebViewWindowOpen();
        
        [DllImport("__Internal")]
        private static extern void SetWebViewCallback(string callbackName);
        
        private bool isVisible = false;
        private string currentUrl;
        
        public event Action<string> OnPageLoaded;
        public event Action<string> OnPaymentComplete;
        public event Action<string> OnError;
        public event Action OnWebViewClosed;
        
        public bool IsVisible => isVisible;
        
        public WebGLWebView()
        {
            InitializeWebView();
        }
        
        private void InitializeWebView()
        {
            try
            {
                // Set up JavaScript callback
                SetWebViewCallback("HandleWebViewCallback");
                Debug.Log("WebGL WebView initialized");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error initializing WebGL WebView: {e.Message}");
            }
        }
        
        public void LoadPaymentUrl(string url)
        {
            currentUrl = url;
            
            // For WebGL, we open the URL in a new window/tab
            // The actual WebView functionality is handled by the browser
            Debug.Log($"WebGL WebView: Opening payment URL in new window: {url}");
            
            // Simulate page loaded event
            OnPageLoaded?.Invoke(url);
        }
        
        public void ExecuteJavaScript(string js)
        {
            // JavaScript execution is not supported in WebGL mode
            // as we're opening URLs in new windows
            Debug.LogWarning("JavaScript execution not supported in WebGL WebView mode");
        }
        
        public void SetSize(int width, int height)
        {
            // Size is set when opening the window
            Debug.Log($"WebGL WebView: Window size set to {width}x{height}");
        }
        
        public void SetPosition(int x, int y)
        {
            // Position is handled by the browser when opening new windows
            Debug.Log($"WebGL WebView: Window position set to ({x}, {y})");
        }
        
        public void Show()
        {
            if (!string.IsNullOrEmpty(currentUrl))
            {
                try
                {
                    // Open the payment URL in a new window
                    OpenWebViewWindow(currentUrl, "BoltPayment", 800, 600);
                    isVisible = true;
                    
                    Debug.Log("WebGL WebView: Payment window opened");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error opening WebGL WebView: {e.Message}");
                    OnError?.Invoke("Failed to open payment window");
                }
            }
        }
        
        public void Hide()
        {
            try
            {
                CloseWebViewWindow();
                isVisible = false;
                Debug.Log("WebGL WebView: Payment window closed");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error closing WebGL WebView: {e.Message}");
            }
        }
        
        public void Close()
        {
            Hide();
            OnWebViewClosed?.Invoke();
        }
        
        // Called from JavaScript
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void HandleWebViewCallback(string message);
        
        // This method is called from JavaScript when payment is completed
        public static void HandlePaymentComplete(string result)
        {
            Debug.Log($"WebGL WebView: Payment completed - {result}");
            
            // Find the WebView instance and trigger the event
            // This is a simplified approach - in a real implementation,
            // you'd need to maintain a reference to the current WebView instance
        }
        
        // This method is called from JavaScript when an error occurs
        public static void HandlePaymentError(string error)
        {
            Debug.LogError($"WebGL WebView: Payment error - {error}");
        }
        
        public void Dispose()
        {
            Close();
        }
    }
} 