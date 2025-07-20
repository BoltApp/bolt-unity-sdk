using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

// Unity stubs for standalone testing
namespace UnityEngine
{
    public class MonoBehaviour
    {
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void Update() { }
        protected virtual void OnDestroy() { }

        protected GameObject gameObject = new GameObject("");

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            // Simple stub implementation
            return new Coroutine();
        }

        protected void Destroy(UnityEngine.Object obj) { }
    }

    public class GameObject : Object
    {
        public GameObject(string name) { }
        public T AddComponent<T>() where T : MonoBehaviour
        {
            return Activator.CreateInstance<T>();
        }
    }

    public class Coroutine
    {
        public Coroutine() { }
    }

    public class Object
    {
        public static void DontDestroyOnLoad(Object target) { }
        public static void DontDestroyOnLoad(GameObject target) { }
    }

    public class WaitForSeconds
    {
        public WaitForSeconds(float seconds) { }
    }

    public static class Debug
    {
        public static void Log(string message) { }
        public static void LogError(string message) { }
        public static void LogWarning(string message) { }
    }

    public static class Application
    {
        public static void OpenURL(string url) { }
    }

    public static class PlayerPrefs
    {
        private static readonly Dictionary<string, string> _storage = new();

        public static string GetString(string key, string defaultValue = "")
        {
            return _storage.TryGetValue(key, out string value) ? value : defaultValue;
        }

        public static void SetString(string key, string value)
        {
            _storage[key] = value;
        }

        public static void Save() { }
    }

    namespace Networking
    {
        // Empty namespace for UnityWebRequest compatibility
    }
}

namespace System.Web
{
    public static class HttpUtility
    {
        public static System.Collections.Specialized.NameValueCollection ParseQueryString(string query)
        {
            var collection = new System.Collections.Specialized.NameValueCollection();

            if (string.IsNullOrEmpty(query))
                return collection;

            // Remove the leading '?' if present
            if (query.StartsWith("?"))
                query = query.Substring(1);

            var pairs = query.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = Uri.UnescapeDataString(keyValue[0]);
                    var value = Uri.UnescapeDataString(keyValue[1]);
                    collection[key] = value;
                }
            }

            return collection;
        }
    }
}

