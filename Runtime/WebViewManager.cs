using UnityEngine;
using System;

public class WebViewManager : MonoBehaviour
{
    private UniWebView webView;

    public Action OnWebViewClosed; // External callback

    public void OpenFullScreenWebView(string url)
    {
        if (webView != null) return;

        webView = gameObject.AddComponent<UniWebView>();
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        webView.OnShouldClose += OnShouldClose;
        webView.OnPageFinished += (view, statusCode, urlStr) => Debug.Log("Page Loaded: " + urlStr);
        webView.Load(url);
        webView.Show();
    }

    private bool OnShouldClose(UniWebView webView)
    {
        CloseWebView();
        OnWebViewClosed?.Invoke(); // Fire the close event
        return true;
    }

    public void CloseWebView()
    {
        if (webView != null)
        {
            webView.OnShouldClose -= OnShouldClose;
            webView.Hide();
            Destroy(webView);
            webView = null;
        }
    }
}
