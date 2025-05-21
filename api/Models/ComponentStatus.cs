namespace Api.Models;

public record ComponentStatus
{
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public required string Status { get; set; }
    public string? Description { get; set; }
    public DateTime? LastStatusChange { get; set; }
}
