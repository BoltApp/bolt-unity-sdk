using System;
using Newtonsoft.Json;

namespace BoltApp
{
    /// <summary>
    /// Payload for the bolt-gaming-sdk-openad event posted to the webview.
    /// </summary>
    public class AdOpenEventPayload
    {
        [JsonProperty("ad_placement")]
        public string AdPlacement { get; set; }

        [JsonProperty("buttonID")]
        public string ButtonID { get; set; }

        [JsonProperty("metadata")]
        public AdMetaData Metadata { get; set; }
    }

    /// <summary>
    /// Generalized SDK event structure posted to the ad webview (e.g. open-ad).
    /// </summary>
    public class BoltSdkEvent
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("payload")]
        public object Payload { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        public static BoltSdkEvent CreateAdOpenEvent(string adPlacement, string buttonID, AdMetaData metadata)
        {
            return new BoltSdkEvent
            {
                Type = "bolt-gaming-sdk-openad",
                Payload = new AdOpenEventPayload
                {
                    AdPlacement = adPlacement ?? string.Empty,
                    ButtonID = buttonID ?? string.Empty,
                    Metadata = metadata ?? AdMetaData.New()
                },
                Id = "bolt-event-" + Guid.NewGuid().ToString(),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }
    }
}
