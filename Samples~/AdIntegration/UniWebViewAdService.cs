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
            // Handle onClaim via message handler
            _webView.OnMessageReceived += (view, message) =>
            {
                if (message.Path != null && message.Path.Equals("onClaim", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("[UniWebViewAdService] onClaim message received");
                    _onClaimCallback?.Invoke();
                }
            };

            // Handle onClaim via channel message handler
            _webView.OnChannelMessageReceived += (view, channelMessage) =>
            {
                if (channelMessage.action != null && channelMessage.action.Equals("onClaim", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("[UniWebViewAdService] onClaim channel message received");
                    _onClaimCallback?.Invoke();
                    
                    if (channelMessage.isSyncCall || channelMessage.isAsyncRequest)
                    {
                        return UniWebViewChannelMessageResponse.Success("Claim received");
                    }
                }
                
                return UniWebViewChannelMessageResponse.Success("");
            };
        }
    }
}
#endif
