using System;

namespace BoltApp
{
    /// <summary>
    /// Interface for ad web view operations
    /// </summary>
    public interface IAdWebViewService
    {
        void Preload(string adLink);
        void Show();
        void Cleanup();
        void SetOnClaimCallback(Action onClaimCallback);
    }
}