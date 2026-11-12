using System.ComponentModel.DataAnnotations;

namespace RareCrewCSharp.Configuration;

public class ApiOptions
{
    public const string SectionName = "ApiConfiguration";

    [Required, Url]
    public string BaseUrl { get; set; } = string.Empty;

    [Required]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    public string Key { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int CacheMinutes { get; set; } = 5;
}
