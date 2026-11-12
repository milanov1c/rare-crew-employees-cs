using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RareCrewCSharp.Configuration;
using RareCrewCSharp.DTO;

namespace RareCrewCSharp.Clients;

public class TimeEntryClient : ITimeEntryClient
{
    private const string CacheKey = "time-entries";

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ApiOptions _options;
    private readonly ILogger<TimeEntryClient> _logger;

    public TimeEntryClient(
        HttpClient httpClient,
        IMemoryCache cache,
        IOptions<ApiOptions> options,
        ILogger<TimeEntryClient> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TimeEntryDto>> GetTimeEntriesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out IReadOnlyList<TimeEntryDto>? cached) && cached is not null)
        {
            return cached;
        }

        var requestUri = $"{_options.Endpoint.TrimStart('/')}?code={Uri.EscapeDataString(_options.Key)}";

        _logger.LogInformation("Fetching time entries from {BaseUrl}/{Endpoint}", _options.BaseUrl, _options.Endpoint);

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var entries = await response.Content.ReadFromJsonAsync<List<TimeEntryDto>>(cancellationToken)
                      ?? new List<TimeEntryDto>();

        _logger.LogInformation("Received {Count} time entries", entries.Count);

        _cache.Set(CacheKey, (IReadOnlyList<TimeEntryDto>)entries, TimeSpan.FromMinutes(_options.CacheMinutes));
        return entries;
    }
}
