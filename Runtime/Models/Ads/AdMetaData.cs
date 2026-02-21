using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoltApp
{
    /// <summary>
    /// JSON converter so AdMetaData serializes as its key-value map.
    /// </summary>
    internal class AdMetaDataConverter : JsonConverter<AdMetaData>
    {
        public override void WriteJson(JsonWriter writer, AdMetaData value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            serializer.Serialize(writer, value.GetData());
        }

        public override AdMetaData ReadJson(JsonReader reader, Type objectType, AdMetaData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dict = serializer.Deserialize<Dictionary<string, string>>(reader);
            return dict != null ? AdMetaData.FromDictionary(dict) : AdMetaData.New();
        }
    }

    /// <summary>
    /// Builder class for ad metadata with fluent API
    /// </summary>
    [JsonConverter(typeof(AdMetaDataConverter))]
    public class AdMetaData
    {
        private readonly Dictionary<string, string> _data;

        private AdMetaData()
        {
            _data = new Dictionary<string, string>();
        }

        private AdMetaData(Dictionary<string, string> data)
        {
            _data = data != null ? new Dictionary<string, string>(data) : new Dictionary<string, string>();
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
        /// Creates AdMetaData from a dictionary (used by JSON deserialization).
        /// </summary>
        internal static AdMetaData FromDictionary(Dictionary<string, string> data)
        {
            return new AdMetaData(data);
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

