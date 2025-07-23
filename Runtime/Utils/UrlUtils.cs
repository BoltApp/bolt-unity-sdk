using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BoltSDK
{
    /// <summary>
    /// Utility class for URL operations
    /// </summary>
    public static class UrlUtils
    {
        public static string AppendQueryParameters(string baseUrl, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return baseUrl;

            if (parameters == null || parameters.Count == 0)
                return baseUrl;

            var uriBuilder = new UriBuilder(baseUrl);
            var query = new StringBuilder(uriBuilder.Query);

            foreach (var param in parameters)
            {
                if (query.Length > 0)
                    query.Append('&');
                query.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
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

        public static string GetBaseUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            try
            {
                var uri = new Uri(url);
                return $"{uri.Scheme}://{uri.Authority}{uri.AbsolutePath}";
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get base URL: {ex.Message}");
                return url;
            }
        }

        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Uri.EscapeDataString(value);
        }

        public static string UrlDecode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Uri.UnescapeDataString(value);
        }

        public static string CombineUrl(string baseUrl, string path)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return path;

            if (string.IsNullOrEmpty(path))
                return baseUrl;

            // Remove trailing slash from base URL
            baseUrl = baseUrl.TrimEnd('/');

            // Remove leading slash from path
            path = path.TrimStart('/');

            return $"{baseUrl}/{path}";
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