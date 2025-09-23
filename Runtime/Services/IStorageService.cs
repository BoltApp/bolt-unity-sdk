using System;
using System.Collections.Generic;

namespace BoltApp
{
    /// <summary>
    /// Interface for data storage operations
    /// </summary>
    public interface IStorageService
    {
        void SetString(string key, string value);
        string GetString(string key, string defaultValue = "");
        void SetInt(string key, int value);
        int GetInt(string key, int defaultValue = 0);
        void SetBool(string key, bool value);
        bool GetBool(string key, bool defaultValue = false);
        void SetObject<T>(string key, T obj);
        T GetObject<T>(string key, T defaultValue = default(T));
        void SetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> dictionary);
        Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> defaultValue = null);
        bool HasKey(string key);
        void DeleteKey(string key);
        void DeleteAll();
        void Save();
    }
}