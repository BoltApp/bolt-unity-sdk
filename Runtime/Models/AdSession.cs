using System;
using System.Threading.Tasks;

namespace BoltApp
{
    /// <summary>
    /// Light wrapper around an ad object used in tracking recent ads
    /// </summary>
    [Serializable]
    public class AdSession
    {
        public string AdLink;
        public AdType Type;
        public AdStatus Status;
        public DateTime CreatedAt;
        public DateTime CompletedAt;
        public string Error;

        private readonly Action _onShow;
        private readonly Action<string> _onCompleted;

        public AdSession()
        {
            CreatedAt = DateTime.UtcNow;
            Status = AdStatus.Preloaded;
        }

        public AdSession(string adLink, AdType type, Action onShow, Action<string> onCompleted = null)
        {
            AdLink = adLink;
            Type = type;
            Status = AdStatus.Preloaded;
            CreatedAt = DateTime.UtcNow;
            _onShow = onShow;
            _onCompleted = onCompleted;
        }

        /// <summary>
        /// Fires the onCompleted callback
        /// </summary>
        public void FireOnCompleted()
        {
            _onCompleted?.Invoke(AdLink);
        }

        /// <summary>
        /// Show the preloaded advertisement
        /// </summary>
        public Task Show()
        {
            Status = AdStatus.Showing;
            _onShow?.Invoke();
            return Task.CompletedTask;
        }

        public void UpdateStatus(AdStatus status, string error = null)
        {
            Status = status;
            Error = error;
            if (status == AdStatus.Completed || status == AdStatus.Failed)
            {
                CompletedAt = DateTime.UtcNow;
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(AdLink);
        }

        public bool IsSuccess()
        {
            return Status == AdStatus.Completed;
        }

        public override string ToString()
        {
            return $"AdLink: {AdLink}, Type: {Type}, Status: {Status}, CreatedAt: {CreatedAt}, CompletedAt: {CompletedAt}, Error: {Error}";
        }
    }
}
