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
            if (string.IsNullOrEmpty(ServerUrl) || string.IsNullOrEmpty(AppName))
                return false;

            if (!Uri.IsWellFormedUriString(ServerUrl, UriKind.Absolute))
                return false;

            try
            {
                var uri = new Uri(ServerUrl);
                return uri.Scheme == "http" || uri.Scheme == "https";
            }
            catch
            {
                return false;
            }
        }
    }
}