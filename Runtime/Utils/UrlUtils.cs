using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BoltApp
{
    /// <summary>
    /// Utility class for URL operations
    /// </summary>
    public static class UrlUtils
    {

        public static UriBuilder BuildCheckoutLink(string baseUrl, BoltUser boltUser, Dictionary<string, string> extraParams)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return baseUrl;

            var uriBuilder = new UriBuilder(baseUrl);
            var query = new StringBuilder(uriBuilder.Query);

            // Add extra query parameters
            try
            {
                foreach (var param in extraParams)
                {
                    if (query.Length > 0)
                        query.Append('&');
                    query.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BoltSDK] Failed to add extra params to checkout link: {ex.Message}");
            }


            // Add user query parameters
            try
            {
                var userData = boltUser.ToDictionary();
                foreach (var param in userData)
                {
                    if (query.Length > 0)
                        query.Append('&');
                    query.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BoltSDK] Failed to add user data to checkout link: {ex.Message}");
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public static Dictionary<string, string> ExtractQueryParameters(string url)
        {
            var parameters = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(url))
                return parameters;

            try
            {
                var uri = new Uri(url);
                var query = uri.Query;

                if (string.IsNullOrEmpty(query))
                    return parameters;

                // Remove the leading '?'
                query = query.Substring(1);

                var pairs = query.Split('&');
                foreach (var pair in pairs)
                {
                    var keyValue = pair.Split('=');
                    if (keyValue.Length == 2)
                    {
                        var key = Uri.UnescapeDataString(keyValue[0]);
                        var value = Uri.UnescapeDataString(keyValue[1]);
                        parameters[key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to extract query parameters from URL: {ex.Message}");
            }

            return parameters;
        }

        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            try
            {
                var uri = new Uri(url);
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDeepLinkCallback(string url, string scheme)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(scheme))
                return false;

            try
            {
                var uri = new Uri(url);
                return uri.Scheme.Equals(scheme, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}