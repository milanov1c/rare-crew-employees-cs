using System.Text.Json.Serialization;

namespace RareCrewCSharp.DTO;

public class TimeEntryDto
{
    public string? Id { get; set; }

    public string? EmployeeName { get; set; }

    [JsonPropertyName("StarTimeUtc")]
    public DateTime? StartTimeUtc { get; set; }

    [JsonPropertyName("EndTimeUtc")]
    public DateTime? EndTimeUtc { get; set; }

    public string? EntryNotes { get; set; }

    public DateTime? DeletedOn { get; set; }
}
