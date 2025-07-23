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
        /// <summary>
        /// Appends query parameters to a URL
        /// </summary>
        /// <param name="baseUrl">The base URL</param>
        /// <param name="parameters">The parameters to append</param>
        /// <returns>The URL with appended parameters</returns>
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

        /// <summary>
        /// Extracts query parameters from a URL
        /// </summary>
        /// <param name="url">The URL to extract parameters from</param>
        /// <returns>A dictionary of query parameters</returns>
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

        /// <summary>
        /// Validates if a string is a valid URL
        /// </summary>
        /// <param name="url">The URL to validate</param>
        /// <returns>True if the URL is valid, false otherwise</returns>
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

        /// <summary>
        /// Gets the base URL without query parameters
        /// </summary>
        /// <param name="url">The full URL</param>
        /// <returns>The base URL without query parameters</returns>
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

        /// <summary>
        /// Encodes a string for use in a URL
        /// </summary>
        /// <param name="value">The string to encode</param>
        /// <returns>The URL-encoded string</returns>
        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Uri.EscapeDataString(value);
        }

        /// <summary>
        /// Decodes a URL-encoded string
        /// </summary>
        /// <param name="value">The URL-encoded string to decode</param>
        /// <returns>The decoded string</returns>
        public static string UrlDecode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Uri.UnescapeDataString(value);
        }

        /// <summary>
        /// Combines URL parts safely
        /// </summary>
        /// <param name="baseUrl">The base URL</param>
        /// <param name="path">The path to append</param>
        /// <returns>The combined URL</returns>
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

        /// <summary>
        /// Checks if a URL is a deep link callback
        /// </summary>
        /// <param name="url">The URL to check</param>
        /// <param name="scheme">The expected scheme</param>
        /// <returns>True if it's a deep link callback, false otherwise</returns>
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