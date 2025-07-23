using System;
using System.Collections.Generic;

namespace BoltSDK
{
    /// <summary>
    /// Interface for data storage operations
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Stores a string value with the specified key
        /// </summary>
        /// <param name="key">The key to store the value under</param>
        /// <param name="value">The string value to store</param>
        void SetString(string key, string value);

        /// <summary>
        /// Retrieves a string value for the specified key
        /// </summary>
        /// <param name="key">The key to retrieve the value for</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>The stored string value or the default value</returns>
        string GetString(string key, string defaultValue = "");

        /// <summary>
        /// Stores an integer value with the specified key
        /// </summary>
        /// <param name="key">The key to store the value under</param>
        /// <param name="value">The integer value to store</param>
        void SetInt(string key, int value);

        /// <summary>
        /// Retrieves an integer value for the specified key
        /// </summary>
        /// <param name="key">The key to retrieve the value for</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>The stored integer value or the default value</returns>
        int GetInt(string key, int defaultValue = 0);

        /// <summary>
        /// Stores a boolean value with the specified key
        /// </summary>
        /// <param name="key">The key to store the value under</param>
        /// <param name="value">The boolean value to store</param>
        void SetBool(string key, bool value);

        /// <summary>
        /// Retrieves a boolean value for the specified key
        /// </summary>
        /// <param name="key">The key to retrieve the value for</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>The stored boolean value or the default value</returns>
        bool GetBool(string key, bool defaultValue = false);

        /// <summary>
        /// Stores a JSON-serializable object with the specified key
        /// </summary>
        /// <param name="key">The key to store the object under</param>
        /// <param name="obj">The object to store (must be serializable)</param>
        void SetObject<T>(string key, T obj);

        /// <summary>
        /// Retrieves a JSON-serializable object for the specified key
        /// </summary>
        /// <param name="key">The key to retrieve the object for</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>The stored object or the default value</returns>
        T GetObject<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Checks if a key exists in storage
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key exists, false otherwise</returns>
        bool HasKey(string key);

        /// <summary>
        /// Removes a key-value pair from storage
        /// </summary>
        /// <param name="key">The key to remove</param>
        void DeleteKey(string key);

        /// <summary>
        /// Removes all stored data
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Saves any pending changes to persistent storage
        /// </summary>
        void Save();
    }
}