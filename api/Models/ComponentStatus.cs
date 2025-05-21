namespace Api.Models;

public record ComponentStatus
{
    public required string Name { get; set; } = string.Empty;
    public required string Status { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime? LastUpdated { get; set; }
}
