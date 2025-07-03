using UnityEngine;
using System;
using UnityEngine.UI;

public class WebViewManager : MonoBehaviour
{
    private WebViewObject webViewObject;
    public Action OnWebViewClosed;

    /// <summary>
    /// Opens a full-screen web view with the given URL.
    /// </summary>
    /// <param name="url">The URL to load in the web view.</param>
    public void OpenFullScreenWebView(string url)
    {
        if (webViewObject != null) return;

        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
            },
            started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
            },
            enableWKWebView: true);

        webViewObject.SetMargins(0, 0, 0, 0);
        webViewObject.SetVisibility(true);
        webViewObject.LoadURL(url);
    }

    public void CloseWebView()
    {
        if (webViewObject != null)
        {
            webViewObject.SetVisibility(false);
            Destroy(webViewObject.gameObject);
            webViewObject = null;
            OnWebViewClosed?.Invoke();
        }
    }

    private void OnDestroy()
    {
        CloseWebView();
    }
}