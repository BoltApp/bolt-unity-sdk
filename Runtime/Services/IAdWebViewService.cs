using System;

namespace BoltApp
{
    /// <summary>
    /// Interface for ad web view operations
    /// Game must implement this interface using UniWebView: https://docs.uniwebview.com/guide/installation.html
    /// </summary>
    public interface IAdWebViewService
    {
        void Initialize();
        void Show(string adLink);
        void Cleanup();
        void SetOnClaimCallback(Action onClaimCallback);
    }
}
