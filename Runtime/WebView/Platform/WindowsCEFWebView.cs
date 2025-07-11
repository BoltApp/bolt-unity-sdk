using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace BoltSDK
{
    public class WindowsCEFWebView : IBoltWebView
    {
        // Native plugin imports
        [DllImport("BoltWebViewPlugin")]
        private static extern IntPtr CreateWebView(int x, int y, int width, int height);
        
        [DllImport("BoltWebViewPlugin")]
        private static extern void DestroyWebView(IntPtr webView);
        
        [DllImport("BoltWebViewPlugin")]
        private static extern void LoadURL(IntPtr webView, string url);
        
        [DllImport("BoltWebViewPlugin")]
        private static extern void ExecuteJavaScript(IntPtr webView, string js);
        
        [DllImport("BoltWebViewPlugin")]
        private static extern void SetVisibility(IntPtr webView, bool visible);
        
        [DllImport("BoltWebViewPlugin")]
        private static extern void SetPosition(IntPtr webView, int x, int y);
        
        [DllImport("BoltWebViewPlugin")]
        private static extern void SetSize(IntPtr webView, int width, int height);
        
        [DllImport("BoltWebViewPlugin")]
        private static extern void SetCallback(IntPtr webView, WebViewCallback callback);
        
        // Callback delegate
        public delegate void WebViewCallback(string message, int callbackType);
        
        private IntPtr webViewHandle;
        private bool isVisible = false;
        private int currentWidth;
        private int currentHeight;
        private int currentX;
        private int currentY;
        
        public event Action<string> OnPageLoaded;
        public event Action<string> OnPaymentComplete;
        public event Action<string> OnError;
        public event Action OnWebViewClosed;
        
        public bool IsVisible => isVisible;
        
        public WindowsCEFWebView()
        {
            InitializeWebView();
        }
        
        private void InitializeWebView()
        {
            try
            {
                // Create WebView with initial size
                webViewHandle = CreateWebView(0, 0, 800, 600);
                
                if (webViewHandle != IntPtr.Zero)
                {
                    // Set up callback
                    SetCallback(webViewHandle, HandleNativeCallback);
                    Debug.Log("Windows CEF WebView initialized successfully");
                }
                else
                {
                    Debug.LogError("Failed to create Windows CEF WebView");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error initializing Windows CEF WebView: {e.Message}");
                // Fallback to Unity WebView if CEF is not available
                FallbackToUnityWebView();
            }
        }
        
        private void FallbackToUnityWebView()
        {
            Debug.LogWarning("CEF not available, falling back to Unity WebView");
            // This would require creating a UnityWebView instance
            // For now, we'll just log the error
        }
        
        public void LoadPaymentUrl(string url)
        {
            if (webViewHandle != IntPtr.Zero)
            {
                LoadURL(webViewHandle, url);
            }
            else
            {
                Debug.LogError("WebView not initialized");
            }
        }
        
        public void ExecuteJavaScript(string js)
        {
            if (webViewHandle != IntPtr.Zero && isVisible)
            {
                ExecuteJavaScript(webViewHandle, js);
            }
        }
        
        public void SetSize(int width, int height)
        {
            currentWidth = width;
            currentHeight = height;
            
            if (webViewHandle != IntPtr.Zero)
            {
                SetSize(webViewHandle, width, height);
            }
        }
        
        public void SetPosition(int x, int y)
        {
            currentX = x;
            currentY = y;
            
            if (webViewHandle != IntPtr.Zero)
            {
                SetPosition(webViewHandle, x, y);
            }
        }
        
        public void Show()
        {
            if (webViewHandle != IntPtr.Zero)
            {
                SetVisibility(webViewHandle, true);
                isVisible = true;
            }
        }
        
        public void Hide()
        {
            if (webViewHandle != IntPtr.Zero)
            {
                SetVisibility(webViewHandle, false);
                isVisible = false;
            }
        }
        
        public void Close()
        {
            Hide();
            OnWebViewClosed?.Invoke();
        }
        
        private void HandleNativeCallback(string message, int callbackType)
        {
            switch (callbackType)
            {
                case 0: // Page loaded
                    OnPageLoaded?.Invoke(message);
                    break;
                case 1: // Payment complete
                    OnPaymentComplete?.Invoke(message);
                    break;
                case 2: // Error
                    OnError?.Invoke(message);
                    break;
                case 3: // WebView closed
                    OnWebViewClosed?.Invoke();
                    break;
                default:
                    Debug.Log($"WebView callback: {message} (type: {callbackType})");
                    break;
            }
        }
        
        public void Dispose()
        {
            if (webViewHandle != IntPtr.Zero)
            {
                DestroyWebView(webViewHandle);
                webViewHandle = IntPtr.Zero;
            }
        }
    }
} 