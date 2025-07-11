using System;

namespace BoltSDK
{
    public interface IBoltWebView : IDisposable
    {
        event Action<string> OnPageLoaded;
        event Action<string> OnPaymentComplete;
        event Action<string> OnError;
        event Action OnWebViewClosed;
        
        void LoadPaymentUrl(string url);
        void ExecuteJavaScript(string js);
        void SetSize(int width, int height);
        void SetPosition(int x, int y);
        void Show();
        void Hide();
        void Close();
        bool IsVisible { get; }
    }
} 