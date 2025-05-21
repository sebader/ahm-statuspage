using System.Text.Json.Serialization;

namespace Api.Models
{
    public record HealthEngineResponse
    {
        [JsonPropertyName("healthModel")]
        public required HealthModel healthModel { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata? metadata { get; set; }

        [JsonPropertyName("errors")]
        public object? errors { get; set; }
    }

    public record HealthModel
    {
        [JsonPropertyName("entities")]
        public List<Entity>? entities { get; set; }

        [JsonPropertyName("relationships")]
        public List<Relationship>? relationships { get; set; }
    }

    public record Entity
    {
        [JsonPropertyName("name")]
        public required string name { get; set; }

        [JsonPropertyName("kind")]
        public required string kind { get; set; }

        [JsonPropertyName("locationId")]
        public string? locationId { get; set; }

        [JsonPropertyName("displayName")]
        public string? displayName { get; set; }

        [JsonPropertyName("state")]
        public required string state { get; set; }

        [JsonPropertyName("createdTime")]
        public required string createdTime { get; set; }

        [JsonPropertyName("lastTransitionTimeUtc")]
        public string? lastTransitionTimeUtc { get; set; }

        [JsonPropertyName("deletedTime")]
        public string? deletedTime { get; set; }

        [JsonPropertyName("labels")]
        public Dictionary<string, string>? labels { get; set; }

        [JsonPropertyName("impact")]
        public required string impact { get; set; }
    }

    public record Relationship
    {
        [JsonPropertyName("name")]
        public required string name { get; set; }

        [JsonPropertyName("kind")]
        public string? kind { get; set; }

        [JsonPropertyName("displayName")]
        public string? displayName { get; set; }

        [JsonPropertyName("from")]
        public required string from { get; set; }

        [JsonPropertyName("to")]
        public required string to { get; set; }

        [JsonPropertyName("createdTime")]
        public required string createdTime { get; set; }

        [JsonPropertyName("deletedTime")]
        public string? deletedTime { get; set; }
    }

    public record Metadata
    {
        [JsonPropertyName("timeStampUtc")]
        public string? timeStampUtc { get; set; }

        [JsonPropertyName("totalServerTimeInMs")]
        public int? totalServerTimeInMs { get; set; }
    }
}
