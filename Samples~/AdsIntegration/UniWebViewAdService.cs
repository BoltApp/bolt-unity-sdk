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
        private string _adHost;

        public void Preload(string adLink)
        {
            // TODO: @aweislow remove
            Debug.Log($"[UniWebViewAdService] Preload URL: {adLink}");
            if (_webView != null)
            {
                _webView.Load(adLink);
                return;
            }

            _adHost = new Uri(adLink).Host;

            UniWebView.SetAllowAutoPlay(true);
            UniWebView.SetAllowInlinePlay(true);

            var webViewGameObject = new GameObject("BoltAdWebView");
            _webView = webViewGameObject.AddComponent<UniWebView>();
            _webView.Frame = new Rect(0, 0, Screen.width, Screen.height);

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

        public void PostWebviewMessage(BoltSdkEvent eventData)
        {
            if (_webView == null || eventData == null) return;
            var eventJson = eventData.ToJson();
            var escaped = eventJson.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", "\\r").Replace("\n", "\\n");
            var script = "window.postMessage(JSON.parse('" + escaped + "'), '*');";
            _webView.EvaluateJavaScript(script, (payload) =>
            {
                if (payload == null || !payload.resultCode.Equals("0"))
                    Debug.LogWarning("[UniWebViewAdService] PostWebviewMessage EvaluateJavaScript failed: " + (payload?.data ?? "null"));
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
                if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Host != _adHost)
                {
                    view.Stop();
                    Application.OpenURL(url);
                    Debug.Log($"[UniWebViewAdService] External link opened in system browser: {url}");
                }
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