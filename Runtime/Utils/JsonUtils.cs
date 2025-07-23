using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoltSDK
{
    /// <summary>
    /// Utility class for JSON operations
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// Serializes an object to JSON string
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The JSON string representation</returns>
        public static string ToJson(object obj)
        {
            try
            {
                return JsonUtility.ToJson(obj);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to serialize object to JSON: {ex.Message}");
                return "{}";
            }
        }

        /// <summary>
        /// Deserializes a JSON string to an object
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="json">The JSON string to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T FromJson<T>(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                    return default(T);

                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize JSON: {ex.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// Deserializes a JSON string to an object with error handling
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="json">The JSON string to deserialize</param>
        /// <param name="result">The deserialized object (output parameter)</param>
        /// <returns>True if deserialization was successful, false otherwise</returns>
        public static bool TryFromJson<T>(string json, out T result)
        {
            result = default(T);

            try
            {
                if (string.IsNullOrEmpty(json))
                    return false;

                result = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize JSON: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Serializes a dictionary to JSON string
        /// </summary>
        /// <param name="dictionary">The dictionary to serialize</param>
        /// <returns>The JSON string representation</returns>
        public static string DictionaryToJson(Dictionary<string, object> dictionary)
        {
            try
            {
                var wrapper = new DictionaryWrapper { data = dictionary };
                return JsonUtility.ToJson(wrapper);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to serialize dictionary to JSON: {ex.Message}");
                return "{}";
            }
        }

        /// <summary>
        /// Deserializes a JSON string to a dictionary
        /// </summary>
        /// <param name="json">The JSON string to deserialize</param>
        /// <returns>The deserialized dictionary</returns>
        public static Dictionary<string, object> JsonToDictionary(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                    return new Dictionary<string, object>();

                var wrapper = JsonUtility.FromJson<DictionaryWrapper>(json);
                return wrapper?.data ?? new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize JSON to dictionary: {ex.Message}");
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Pretty prints a JSON string with proper indentation
        /// </summary>
        /// <param name="json">The JSON string to format</param>
        /// <returns>The formatted JSON string</returns>
        public static string PrettyPrint(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                    return json;

                // Simple pretty printing - in a real implementation you might want to use a more robust solution
                return json.Replace(",", ",\n  ").Replace("{", "{\n  ").Replace("}", "\n}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to pretty print JSON: {ex.Message}");
                return json;
            }
        }

        /// <summary>
        /// Wrapper class for serializing dictionaries with Unity's JsonUtility
        /// </summary>
        [Serializable]
        private class DictionaryWrapper
        {
            public Dictionary<string, object> data;
        }
    }
}