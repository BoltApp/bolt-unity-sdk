using UnityEngine;

namespace BoltSDK
{
    public static class WebViewFactory
    {
        public static string GetCurrentPlatform()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return "Windows";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            return "macOS";
#elif UNITY_IOS
            return "iOS";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_WEBGL
            return "WebGL";
#elif UNITY_EDITOR_LINUX
            return "Linux";
#else
            return "Unknown";
#endif
        }
        
        public static string GetWebViewType()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return "CEF (Chromium Embedded Framework)";
#elif UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            return "Unity WebView";
#elif UNITY_WEBGL
            return "Browser Window";
#else
            return "Unity WebView (Fallback)";
#endif
        }
        
        public static IBoltWebView CreateWebView()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return new WindowsCEFWebView();
#elif UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            return new UnityWebView();
#elif UNITY_WEBGL
            return new WebGLWebView();
#elif UNITY_EDITOR_LINUX
            return new UnityWebView();
#else
            // Fallback to Unity WebView for other platforms
            return new UnityWebView();
#endif
        }
    }
} 