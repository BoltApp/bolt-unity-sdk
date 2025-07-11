using UnityEngine;
using System;
using System.Collections;

namespace BoltSDK
{
    public class UnityWebView : IBoltWebView
    {
        private WebViewObject webViewObject;
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
        
        public UnityWebView()
        {
            InitializeWebView();
        }
        
        private void InitializeWebView()
        {
            GameObject webViewGO = new GameObject("WebViewObject");
            webViewObject = webViewGO.AddComponent<WebViewObject>();
            
            webViewObject.Init(
                cb: (msg) => HandleJavaScriptCallback(msg),
                err: (msg) => HandleError(msg),
                started: (msg) => Debug.Log($"WebView started: {msg}"),
                ld: (msg) => HandlePageLoaded(msg),
                enableWKWebView: true
            );
            
            // Inject device pixel ratio for proper mobile rendering
            InjectDevicePixelRatio();
        }
        
        public void LoadPaymentUrl(string url)
        {
            if (webViewObject == null)
            {
                Debug.LogError("WebView not initialized");
                return;
            }
            
            // Inject viewport meta tag for mobile devices
            if (IsMobileDevice())
            {
                url = InjectViewportMetaTag(url);
            }
            
            webViewObject.LoadURL(url);
        }
        
        public void ExecuteJavaScript(string js)
        {
            if (webViewObject != null && isVisible)
            {
                webViewObject.EvaluateJS(js);
            }
        }
        
        public void SetSize(int width, int height)
        {
            currentWidth = width;
            currentHeight = height;
            
            if (webViewObject != null)
            {
                webViewObject.SetMargins(0, 0, 0, 0);
                webViewObject.SetVisibility(false);
                
                // Set the size by adjusting margins
                int screenWidth = Screen.width;
                int screenHeight = Screen.height;
                
                int leftMargin = (screenWidth - width) / 2;
                int topMargin = (screenHeight - height) / 2;
                int rightMargin = screenWidth - (leftMargin + width);
                int bottomMargin = screenHeight - (topMargin + height);
                
                webViewObject.SetMargins(leftMargin, topMargin, rightMargin, bottomMargin);
            }
        }
        
        public void SetPosition(int x, int y)
        {
            currentX = x;
            currentY = y;
            
            if (webViewObject != null)
            {
                int screenWidth = Screen.width;
                int screenHeight = Screen.height;
                
                int leftMargin = x;
                int topMargin = y;
                int rightMargin = screenWidth - (x + currentWidth);
                int bottomMargin = screenHeight - (y + currentHeight);
                
                webViewObject.SetMargins(leftMargin, topMargin, rightMargin, bottomMargin);
            }
        }
        
        public void Show()
        {
            if (webViewObject != null)
            {
                webViewObject.SetVisibility(true);
                isVisible = true;
            }
        }
        
        public void Hide()
        {
            if (webViewObject != null)
            {
                webViewObject.SetVisibility(false);
                isVisible = false;
            }
        }
        
        public void Close()
        {
            Hide();
            OnWebViewClosed?.Invoke();
        }
        
        private void HandleJavaScriptCallback(string message)
        {
            Debug.Log($"WebView JS Callback: {message}");
            
            // Check for payment completion messages
            if (message.Contains("payment_complete") || message.Contains("success"))
            {
                OnPaymentComplete?.Invoke(message);
            }
        }
        
        private void HandleError(string error)
        {
            Debug.LogError($"WebView Error: {error}");
            OnError?.Invoke(error);
        }
        
        private void HandlePageLoaded(string url)
        {
            Debug.Log($"WebView Page Loaded: {url}");
            OnPageLoaded?.Invoke(url);
            
            // Inject additional JavaScript for payment flow
            if (IsMobileDevice())
            {
                InjectMobileOptimizations();
            }
        }
        
        private void InjectDevicePixelRatio()
        {
            if (webViewObject != null)
            {
                int pixelRatio = DetermineDevicePixelRatio();
                string js = $"document.documentElement.style.zoom = {1.0f / pixelRatio};";
                webViewObject.EvaluateJS(js);
            }
        }
        
        private int DetermineDevicePixelRatio()
        {
#if UNITY_IOS
            return 2; // iOS devices typically have 2x pixel ratio
#elif UNITY_ANDROID
            return 2; // Android devices typically have 2x pixel ratio
#else
            return 1; // Desktop typically has 1x pixel ratio
#endif
        }
        
        private string InjectViewportMetaTag(string url)
        {
            // For local URLs, we'll inject the viewport meta tag via JavaScript
            // For remote URLs, we assume they have proper viewport meta tags
            return url;
        }
        
        private void InjectMobileOptimizations()
        {
            if (webViewObject != null)
            {
                string js = @"
                    // Prevent zooming on mobile
                    var viewport = document.querySelector('meta[name=""viewport""]');
                    if (!viewport) {
                        viewport = document.createElement('meta');
                        viewport.name = 'viewport';
                        document.head.appendChild(viewport);
                    }
                    viewport.content = 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no';
                    
                    // Disable text selection
                    document.body.style.webkitUserSelect = 'none';
                    document.body.style.userSelect = 'none';
                    
                    // Prevent pull-to-refresh
                    document.body.style.overscrollBehavior = 'none';
                ";
                
                webViewObject.EvaluateJS(js);
            }
        }
        
        private bool IsMobileDevice()
        {
#if UNITY_IOS || UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }
        
        public void Dispose()
        {
            if (webViewObject != null)
            {
                webViewObject.SetVisibility(false);
                UnityEngine.Object.Destroy(webViewObject.gameObject);
                webViewObject = null;
            }
        }
    }
} 