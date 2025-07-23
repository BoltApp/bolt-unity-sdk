using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoltSDK
{
    /// <summary>
    /// Unity PlayerPrefs implementation of the storage service
    /// </summary>
    public class PlayerPrefsStorageService : IStorageService
    {
        private const string PREFIX = BoltPlayerPrefsKeys.PREFIX;

        public void SetString(string key, string value)
        {
            try
            {
                PlayerPrefs.SetString(GetPrefixedKey(key), value);
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to set string for key '{key}': {ex.Message}");
            }
        }

        public string GetString(string key, string defaultValue = "")
        {
            try
            {
                return PlayerPrefs.GetString(GetPrefixedKey(key), defaultValue);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get string for key '{key}': {ex.Message}");
                return defaultValue;
            }
        }

        public void SetInt(string key, int value)
        {
            try
            {
                PlayerPrefs.SetInt(GetPrefixedKey(key), value);
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to set int for key '{key}': {ex.Message}");
            }
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            try
            {
                return PlayerPrefs.GetInt(GetPrefixedKey(key), defaultValue);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get int for key '{key}': {ex.Message}");
                return defaultValue;
            }
        }

        public void SetBool(string key, bool value)
        {
            try
            {
                PlayerPrefs.SetInt(GetPrefixedKey(key), value ? 1 : 0);
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to set bool for key '{key}': {ex.Message}");
            }
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            try
            {
                return PlayerPrefs.GetInt(GetPrefixedKey(key), defaultValue ? 1 : 0) == 1;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get bool for key '{key}': {ex.Message}");
                return defaultValue;
            }
        }

        public void SetObject<T>(string key, T obj)
        {
            try
            {
                var json = JsonUtils.ToJson(obj);
                SetString(key, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to set object for key '{key}': {ex.Message}");
            }
        }

        public T GetObject<T>(string key, T defaultValue = default(T))
        {
            try
            {
                var json = GetString(key, "");
                if (string.IsNullOrEmpty(json))
                    return defaultValue;

                return JsonUtils.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get object for key '{key}': {ex.Message}");
                return defaultValue;
            }
        }

        public bool HasKey(string key)
        {
            try
            {
                return PlayerPrefs.HasKey(GetPrefixedKey(key));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to check if key '{key}' exists: {ex.Message}");
                return false;
            }
        }

        public void DeleteKey(string key)
        {
            try
            {
                PlayerPrefs.DeleteKey(GetPrefixedKey(key));
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete key '{key}': {ex.Message}");
            }
        }

        public void DeleteAll()
        {
            try
            {
                // Get all keys and delete only Bolt SDK related ones
                var keys = new List<string>();

                // This is a simplified approach - in a real implementation you might want to track keys
                // or use a different approach to delete only Bolt SDK related keys
                foreach (var key in GetAllBoltKeys())
                {
                    PlayerPrefs.DeleteKey(key);
                }

                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete all Bolt SDK data: {ex.Message}");
            }
        }

        public void Save()
        {
            try
            {
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save PlayerPrefs: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a prefixed key to avoid conflicts with other systems
        /// </summary>
        /// <param name="key">The original key</param>
        /// <returns>The prefixed key</returns>
        private string GetPrefixedKey(string key)
        {
            return $"{PREFIX}{key}";
        }

        /// <summary>
        /// Gets all Bolt SDK related keys from PlayerPrefs
        /// </summary>
        /// <returns>List of Bolt SDK keys</returns>
        private List<string> GetAllBoltKeys()
        {
            var keys = new List<string>();

            // This is a simplified implementation
            // In a real implementation, you might want to track keys or use a different approach
            var commonKeys = new[]
            {
                "gameId",
                "userEmail",
                "userLocale",
                "userCountry",
                "deviceId",
                "lastTransactionId",
                "pendingTransactions",
                "acknowledgedTransactions",
                "catalogData",
                "sessionId"
            };

            foreach (var key in commonKeys)
            {
                var prefixedKey = GetPrefixedKey(key);
                if (PlayerPrefs.HasKey(prefixedKey))
                {
                    keys.Add(prefixedKey);
                }
            }

            return keys;
        }

        /// <summary>
        /// Clears all data for testing purposes
        /// </summary>
        public void ClearAllForTesting()
        {
            DeleteAll();
        }
    }
}