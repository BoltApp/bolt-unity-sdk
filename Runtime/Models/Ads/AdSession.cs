using System;

namespace BoltApp
{
    /// <summary>
    /// Light wrapper around an ad object used for tracking progress within Unity's webview
    /// </summary>
    [Serializable]
    public class AdSession
    {
        public string AdLink;
        public AdStatus Status;
        public DateTime CreatedAt;
        public DateTime CompletedAt;
        public string Error;
        public string ButtonID;
        public AdPlacement Placement;
        public AdMetaData Metadata;

        private readonly Action<string> _onCompleted;

        public AdSession()
        {
            CreatedAt = DateTime.UtcNow;
            Status = AdStatus.Preloaded;
        }

        public AdSession(string adLink, Action<string> onCompleted = null)
        {
            AdLink = adLink;
            Status = AdStatus.Preloaded;
            CreatedAt = DateTime.UtcNow;
            _onCompleted = onCompleted;
        }

        public void FireOnCompleted()
        {
            _onCompleted?.Invoke(AdLink);
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
            return $"AdLink: {AdLink}, Status: {Status}, CreatedAt: {CreatedAt}, CompletedAt: {CompletedAt}, Error: {Error}";
        }
    }
}