namespace Api.Models;

using System.Text.Json.Serialization;

public record StateTransition
{
    [JsonPropertyName("previousState")]
    public required string PreviousState { get; set; }
    
    [JsonPropertyName("newState")]
    public required string NewState { get; set; }
    
    [JsonPropertyName("occurrenceTimeUtc")]
    public required DateTime OccurrenceTimeUtc { get; set; }
    
    [JsonPropertyName("labels")]
    public Dictionary<string, string> Labels { get; set; } = new();
    
    [JsonPropertyName("eventId")]
    public string? EventId { get; set; }
}

public record EntityHistory
{
    [JsonPropertyName("entityName")]
    public required string EntityName { get; set; }
    
    [JsonPropertyName("history")]
    public required EntityHistoryData History { get; set; }
}

public record EntityHistoryData
{
    [JsonPropertyName("transitions")]
    public required List<StateTransition> Transitions { get; set; }
}
