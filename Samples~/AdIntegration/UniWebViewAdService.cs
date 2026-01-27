#if UNIWEBVIEW
using System;
using UnityEngine;
using BoltApp;

namespace BoltApp.Samples
{
    /// <summary>
    /// Example implementation of IAdWebViewService using UniWebView
    /// Copy this to your game code and implement IAdWebViewService
    /// </summary>
    public class UniWebViewAdService : IAdWebViewService
    {
        private UniWebView _webView;
        private Action _onClaimCallback;

        public void Initialize()
        {
            if (_webView != null)
            {
                return; // Already initialized
            }

            UniWebView.SetAllowAutoPlay(true);
            UniWebView.SetAllowInlinePlay(true);

            var webViewGameObject = new GameObject("BoltAdWebView");
            _webView = webViewGameObject.AddComponent<UniWebView>();
            _webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            
            Debug.Log("[UniWebViewAdService] Setting SetOpenLinksInExternalBrowser(true)");
            _webView.SetOpenLinksInExternalBrowser(true);
            Debug.Log("[UniWebViewAdService] SetOpenLinksInExternalBrowser configured");

            UniWebView.SetJavaScriptEnabled(true);
            UniWebView.SetForwardWebConsoleToNativeOutput(true);

            SetupEventHandlers();
        }

        public void Show(string adLink)
        {
            if (_webView == null)
            {
                Debug.LogError("[UniWebViewAdService] WebView not initialized. Call Initialize() first.");
                return;
            }

            _webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            _webView.Load(adLink);
            _webView.Show();
        }

        public void Cleanup()
        {
            if (_webView != null)
            {
                _webView.Stop();
                _webView.Hide();
            }
        }

        public void SetOnClaimCallback(Action onClaimCallback)
        {
            _onClaimCallback = onClaimCallback;
        }

        private void SetupEventHandlers()
        {
            _webView.OnPageErrorReceived += (view, errorCode, message) =>
            {
                Debug.LogError($"[UniWebViewAdService] Page error: {errorCode} - {message}");
            };

            _webView.OnPageStarted += (view, url) =>
            {
                Debug.Log($"[UniWebViewAdService] Page started loading: {url}");
            };

            _webView.OnPageCommitted += (view, url) =>
            {
                Debug.Log($"[UniWebViewAdService] Page committed (navigation): {url}");
            };

            _webView.OnPageFinished += (view, statusCode, url) =>
            {
                Debug.Log($"[UniWebViewAdService] Page finished loading: {url} (status: {statusCode})");
            };

            _webView.OnShouldClose += (view) =>
            {
                return false;
            };

            _webView.OnChannelMessageReceived += (view, channelMessage) =>
            {
                // Check for bolt-gaming-issue-reward from UniWebView channel API
                if (channelMessage.action != null && channelMessage.action.Equals("bolt-gaming-issue-reward", StringComparison.OrdinalIgnoreCase))
                {
                    _onClaimCallback?.Invoke();
                }
                
                return null;
            };
        }
    }
}
#endif
