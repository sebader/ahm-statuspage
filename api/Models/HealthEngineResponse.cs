using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Api.Models
{
    public class HealthEngineResponse
    {
        [JsonPropertyName("healthModel")]
        public HealthModel healthModel { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata metadata { get; set; }

        [JsonPropertyName("errors")]
        public object errors { get; set; }
    }

    public class HealthModel
    {
        [JsonPropertyName("entities")]
        public List<Entity> entities { get; set; }

        [JsonPropertyName("relationships")]
        public List<Relationship> relationships { get; set; }
    }

    public class Entity
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("kind")]
        public string kind { get; set; }

        [JsonPropertyName("locationId")]
        public string locationId { get; set; }

        [JsonPropertyName("displayName")]
        public string displayName { get; set; }

        [JsonPropertyName("state")]
        public string state { get; set; }

        [JsonPropertyName("createdTime")]
        public string createdTime { get; set; }

        [JsonPropertyName("lastTransitionTimeUtc")]
        public string lastTransitionTimeUtc { get; set; }

        [JsonPropertyName("deletedTime")]
        public string deletedTime { get; set; }

        [JsonPropertyName("labels")]
        public Dictionary<string, string> labels { get; set; }

        [JsonPropertyName("impact")]
        public string impact { get; set; }
    }

    public class Relationship
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("kind")]
        public string kind { get; set; }

        [JsonPropertyName("displayName")]
        public string displayName { get; set; }

        [JsonPropertyName("from")]
        public string from { get; set; }

        [JsonPropertyName("to")]
        public string to { get; set; }

        [JsonPropertyName("createdTime")]
        public string createdTime { get; set; }

        [JsonPropertyName("deletedTime")]
        public string deletedTime { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("timeStampUtc")]
        public string timeStampUtc { get; set; }

        [JsonPropertyName("totalServerTimeInMs")]
        public int totalServerTimeInMs { get; set; }
    }
}
