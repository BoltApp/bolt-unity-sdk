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

        public static string BuildCheckoutLink(string baseUrl, BoltConfig config, BoltUser boltUser)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return baseUrl;

            var uriBuilder = new UriBuilder(baseUrl);
            var query = new StringBuilder(uriBuilder.Query);

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

            // Add game id and publishable key
            try
            {
                if (!query.ToString().Contains("game_id"))
                {
                    query.Append($"&game_id={config.gameId}");
                }

                if (!query.ToString().Contains("publishable_key"))
                {
                    query.Append($"&publishable_key={config.publishableKey}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BoltSDK] Failed to add app name and publishable key to checkout link: {ex.Message}");
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
    }
}