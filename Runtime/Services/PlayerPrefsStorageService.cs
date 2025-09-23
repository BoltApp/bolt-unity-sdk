using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BoltApp
{
    /// <summary>
    /// Unity PlayerPrefs implementation of the storage service
    /// TODO - This is a temporary implementation, will be replaced with a simpler solution.
    /// </summary>
    public class PlayerPrefsStorageService : IStorageService
    {
        private const string PREFIX = BoltPlayerPrefsKeys.PREFIX;

        public void SetString(string key, string value)
        {
            try
            {
                PlayerPrefs.SetString(key, value);
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
                return PlayerPrefs.GetString(key, defaultValue);
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
                PlayerPrefs.SetInt(key, value);
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
                return PlayerPrefs.GetInt(key, defaultValue);
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
                PlayerPrefs.SetInt(key, value ? 1 : 0);
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
                return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
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
                var json = JsonConvert.SerializeObject(obj, Formatting.None);
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

                return JsonConvert.DeserializeObject<T>(json) ?? defaultValue;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get object for key '{key}': {ex.Message}");
                return defaultValue;
            }
        }

        public void SetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> dictionary)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dictionary, Formatting.None);
                SetString(key, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to set dictionary for key '{key}': {ex.Message}");
            }
        }

        public Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> defaultValue = null)
        {
            try
            {
                var json = GetString(key, "");
                if (string.IsNullOrEmpty(json))
                    return defaultValue ?? new Dictionary<TKey, TValue>();

                return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json) ?? defaultValue ?? new Dictionary<TKey, TValue>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get dictionary for key '{key}': {ex.Message}");
                return defaultValue ?? new Dictionary<TKey, TValue>();
            }
        }

        public bool HasKey(string key)
        {
            try
            {
                return PlayerPrefs.HasKey(key);
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
                PlayerPrefs.DeleteKey(key);
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
                foreach (var key in BoltPlayerPrefsKeys.GetAllBoltKeys())
                {
                    PlayerPrefs.DeleteKey(key);
                }

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

        public void ClearAllForTesting()
        {
            DeleteAll();
        }
    }
}