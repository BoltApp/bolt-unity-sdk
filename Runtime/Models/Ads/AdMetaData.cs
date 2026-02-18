using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoltApp
{
    /// <summary>
    /// Builder class for ad metadata with fluent API
    /// </summary>
    public class AdMetaData
    {
        private readonly Dictionary<string, string> _data;

        private AdMetaData()
        {
            _data = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a new AdMetaData instance
        /// </summary>
        public static AdMetaData New()
        {
            return new AdMetaData();
        }

        /// <summary>
        /// Adds a key-value pair to the metadata. Returns this for method chaining.
        /// </summary>
        public AdMetaData Add(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _data[key] = value ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Serializes the metadata to a JSON string
        /// </summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(_data, Formatting.None);
        }

        /// <summary>
        /// Gets the internal dictionary (for testing/debugging)
        /// </summary>
        internal Dictionary<string, string> GetData()
        {
            return new Dictionary<string, string>(_data);
        }
    }
}

