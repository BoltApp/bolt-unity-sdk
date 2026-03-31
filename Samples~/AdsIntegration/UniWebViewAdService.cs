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
            LoadIframeContent(adLink);
        }

        private void LoadIframeContent(string adLink)
        {
            // Use a clean iframe wrapper to host the ad
            string iframeHtml = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
                    <style>
                        body, html {{
                             margin: 0;
                             padding: 0;
                             width: 100vw;
                             height: 100vh;
                             overflow: hidden;
                             background-color: black;
                        }}
                        iframe {{
                            border: none;
                            width: 100vw;
                            height: 100vh;
                            display: block;
                        }}
                    </style>
                </head>
                <body>
                    <iframe src='{adLink}' id='bolt-iframe-modal' allow='autoplay; fullscreen' allowfullscreen></iframe>
                </body>
                </html>";

            // Use the base URL of the adLink to maintain the same origin for postMessage
            string baseUrl = adLink.Split('?')[0];
            _webView.LoadHTMLString(iframeHtml, baseUrl);
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

            // JS Snippet required to pass messages from the astro ad through uniwebview to the iframe
            var script = $@"
                var iframe = document.getElementById('bolt-iframe-modal');
                if (iframe && iframe.contentWindow) {{
                    iframe.contentWindow.postMessage(JSON.parse('{escaped}'), '*');
                }} else {{
                    window.postMessage(JSON.parse('{escaped}'), '*');
                }}";

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