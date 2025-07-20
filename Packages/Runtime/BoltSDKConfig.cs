using System;

namespace BoltSDK
{
    [Serializable]
    public class BoltSDKConfig
    {
        public string ServerUrl;
        public string AppName;

        public BoltSDKConfig()
        {
            ServerUrl = "";
            AppName = "";
        }

        public BoltSDKConfig(string serverUrl, string appName)
        {
            ServerUrl = serverUrl;
            AppName = appName;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ServerUrl) &&
                   !string.IsNullOrEmpty(AppName) &&
                   Uri.IsWellFormedUriString(ServerUrl, UriKind.Absolute);
        }
    }
}