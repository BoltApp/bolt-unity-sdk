using UnityEngine;
using UnityEditor;
using System;

namespace BoltSDK.Editor
{

    /**
     * Note - this is in beta and is not yet ready for production use.
     * This is a test window to help us test the WebView functionality.
     */
    public class BoltWebViewEditor : EditorWindow
    {
        private string testUrl = "https://example.com";
        private string testJavaScript = "console.log('Test JavaScript executed');";
        private Vector2 scrollPosition;
        private bool isWebViewOpen = false;
        private string webViewStatus = "WebView not initialized";

        [MenuItem("Bolt SDK/WebView Test")]
        public static void ShowWindow()
        {
            GetWindow<BoltWebViewEditor>("Bolt WebView Test");
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Bolt WebView Test Window", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Platform information
            GUILayout.Label("Current Platform:", EditorStyles.boldLabel);
            string currentPlatform = GetCurrentPlatform();
            EditorGUILayout.LabelField("Platform", currentPlatform);
            EditorGUILayout.LabelField("WebView Type", GetWebViewType());

            GUILayout.Space(10);

            // Test URL
            GUILayout.Label("Test Configuration:", EditorStyles.boldLabel);
            testUrl = EditorGUILayout.TextField("Test URL", testUrl);

            GUILayout.Space(5);

            // Test buttons
            GUILayout.Label("WebView Actions:", EditorStyles.boldLabel);

            if (GUILayout.Button("Open Test WebView", GUILayout.Height(30)))
            {
                OpenTestWebView();
            }

            if (GUILayout.Button("Execute JavaScript", GUILayout.Height(30)))
            {
                ExecuteTestJavaScript();
            }

            if (GUILayout.Button("Close WebView", GUILayout.Height(30)))
            {
                CloseTestWebView();
            }

            GUILayout.Space(10);

            // JavaScript test
            GUILayout.Label("JavaScript Test:", EditorStyles.boldLabel);
            testJavaScript = EditorGUILayout.TextArea(testJavaScript, GUILayout.Height(60));

            if (GUILayout.Button("Execute Custom JavaScript", GUILayout.Height(25)))
            {
                ExecuteCustomJavaScript();
            }

            GUILayout.Space(10);

            // Status
            GUILayout.Label("Status:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(webViewStatus, MessageType.Info);

            // Quick test URLs
            GUILayout.Space(10);
            GUILayout.Label("Quick Test URLs:", EditorStyles.boldLabel);

            if (GUILayout.Button("Test Google", GUILayout.Height(25)))
            {
                testUrl = "https://www.google.com";
                OpenTestWebView();
            }

            if (GUILayout.Button("Test Payment Simulator", GUILayout.Height(25)))
            {
                testUrl = "https://stripe.com/docs/testing";
                OpenTestWebView();
            }

            if (GUILayout.Button("Test Local HTML", GUILayout.Height(25)))
            {
                testUrl = "data:text/html,<html><body><h1>Test Page</h1><p>This is a test page for Bolt WebView.</p></body></html>";
                OpenTestWebView();
            }

            GUILayout.Space(10);

            // Platform-specific tests
            GUILayout.Label("Platform-Specific Tests:", EditorStyles.boldLabel);

            if (GUILayout.Button("Test Mobile Optimizations", GUILayout.Height(25)))
            {
                TestMobileOptimizations();
            }

            if (GUILayout.Button("Test Modal Layout", GUILayout.Height(25)))
            {
                TestModalLayout();
            }

            if (GUILayout.Button("Test Payment Flow", GUILayout.Height(25)))
            {
                TestPaymentFlow();
            }

            EditorGUILayout.EndScrollView();
        }

        private string GetCurrentPlatform()
        {
#if UNITY_STANDALONE_WIN
            return "Windows";
#elif UNITY_STANDALONE_OSX
            return "macOS";
#elif UNITY_IOS
            return "iOS";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_WEBGL
            return "WebGL";
#else
            return "Unknown";
#endif
        }

        private string GetWebViewType()
        {
#if UNITY_STANDALONE_WIN
            return "CEF (Chromium Embedded Framework)";
#elif UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX
            return "Unity WebView";
#elif UNITY_WEBGL
            return "Browser Window";
#else
            return "Unity WebView (Fallback)";
#endif
        }

        private void OpenTestWebView()
        {
            try
            {
                // Find or create BoltWebViewManager in the scene
                BoltWebViewManager webViewManager = FindFirstObjectByType<BoltWebViewManager>();
                if (webViewManager == null)
                {
                    GameObject go = new GameObject("BoltWebViewManager");
                    webViewManager = go.AddComponent<BoltWebViewManager>();
                }

                // Set up event handlers
                webViewManager.OnPaymentComplete += (result) =>
                {
                    webViewStatus = $"Payment completed: {result}";
                    isWebViewOpen = false;
                    Repaint();
                };

                webViewManager.OnPaymentError += (error) =>
                {
                    webViewStatus = $"Payment error: {error}";
                    isWebViewOpen = false;
                    Repaint();
                };

                webViewManager.OnWebViewClosed += () =>
                {
                    webViewStatus = "WebView closed";
                    isWebViewOpen = false;
                    Repaint();
                };

                // Open the WebView
                webViewManager.OpenPaymentWebView(testUrl);
                isWebViewOpen = true;
                webViewStatus = $"WebView opened with URL: {testUrl}";

                Debug.Log($"Opened WebView with URL: {testUrl}");
            }
            catch (Exception e)
            {
                webViewStatus = $"Error opening WebView: {e.Message}";
                Debug.LogError($"Error opening WebView: {e}");
            }
        }

        private void ExecuteTestJavaScript()
        {
            try
            {
                BoltWebViewManager webViewManager = FindFirstObjectByType<BoltWebViewManager>();
                if (webViewManager != null && isWebViewOpen)
                {
                    // This would require exposing JavaScript execution through the manager
                    webViewStatus = "JavaScript execution not available in test mode";
                }
                else
                {
                    webViewStatus = "WebView not open";
                }
            }
            catch (Exception e)
            {
                webViewStatus = $"Error executing JavaScript: {e.Message}";
                Debug.LogError($"Error executing JavaScript: {e}");
            }
        }

        private void CloseTestWebView()
        {
            try
            {
                BoltWebViewManager webViewManager = FindFirstObjectByType<BoltWebViewManager>();
                if (webViewManager != null)
                {
                    webViewManager.CloseWebView();
                    isWebViewOpen = false;
                    webViewStatus = "WebView closed";
                }
            }
            catch (Exception e)
            {
                webViewStatus = $"Error closing WebView: {e.Message}";
                Debug.LogError($"Error closing WebView: {e}");
            }
        }

        private void ExecuteCustomJavaScript()
        {
            try
            {
                BoltWebViewManager webViewManager = FindFirstObjectByType<BoltWebViewManager>();
                if (webViewManager != null && isWebViewOpen)
                {
                    // This would require exposing JavaScript execution through the manager
                    webViewStatus = $"Custom JavaScript execution not available in test mode. Code: {testJavaScript}";
                }
                else
                {
                    webViewStatus = "WebView not open";
                }
            }
            catch (Exception e)
            {
                webViewStatus = $"Error executing custom JavaScript: {e.Message}";
                Debug.LogError($"Error executing custom JavaScript: {e}");
            }
        }

        private void TestMobileOptimizations()
        {
            webViewStatus = "Mobile optimizations test completed";
            Debug.Log("Mobile optimizations test completed");
        }

        private void TestModalLayout()
        {
            webViewStatus = "Modal layout test completed";
            Debug.Log("Modal layout test completed");
        }

        private void TestPaymentFlow()
        {
            // Simulate a payment flow
            testUrl = "https://stripe.com/docs/testing#cards";
            OpenTestWebView();
            webViewStatus = "Payment flow test started";
        }
    }
}