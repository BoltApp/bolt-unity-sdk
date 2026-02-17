#if UNIWEBVIEW
using System;
using UnityEngine;
using BoltApp;

namespace BoltApp.Samples // TODO: replace with your own namespace
{
    /// <summary>
    /// Implementation of Bolt SDK's IAdWebViewService interface for UniWebView
    /// Copy this to your game code as-is
    /// </summary>
    public class UniWebViewAdService : IAdWebViewService
    {
        private UniWebView _webView;
        private Action _onClaimCallback;

        public void Preload(string adLink)
        {
            if (_webView != null)
            {
                _webView.Load(adLink);
                return;
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
            _webView.Load(adLink);
        }

        public void Show()
        {
            if (_webView == null)
            {
                Debug.LogError("[UniWebViewAdService] WebView not preloaded.");
                return;
            }

            _webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            _webView.Show();
        }

        public void PostAdShownMetadataEvent(string eventPayloadJson)
        {
            if (_webView == null) return;
            var script = "window.dispatchEvent(new CustomEvent('uniwebview-ad-shown', { detail: " + eventPayloadJson + " }));";
            _webView.EvaluateJavaScript(script, (payload) =>
            {
                if (payload == null || !payload.resultCode.Equals("0"))
                    Debug.LogWarning("[UniWebViewAdService] PostAdShownMetadataEvent EvaluateJavaScript failed: " + (payload?.data ?? "null"));
            });
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