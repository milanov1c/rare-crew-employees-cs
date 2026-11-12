using RareCrewCSharp.Clients;
using RareCrewCSharp.DTO;
using RareCrewCSharp.Models;

namespace RareCrewCSharp.Services;

public class EmployeeReportService : IEmployeeReportService
{
    private readonly ITimeEntryClient _client;

    public EmployeeReportService(ITimeEntryClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<EmployeeSummary>> GetEmployeeSummariesAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _client.GetTimeEntriesAsync(cancellationToken);
        return BuildSummaries(entries);
    }

    private static IReadOnlyList<EmployeeSummary> BuildSummaries(IEnumerable<TimeEntryDto> entries)
    {
        var totals = entries
            .Where(IsValid)
            .GroupBy(e => e.EmployeeName!.Trim())
            .Select(g => new
            {
                Name = g.Key,
                Hours = g.Sum(e => (e.EndTimeUtc!.Value - e.StartTimeUtc!.Value).TotalHours)
            })
            .ToList();

        var grandTotal = totals.Sum(t => t.Hours);

        return totals
            .Select(t => new EmployeeSummary(
                t.Name,
                t.Hours,
                grandTotal > 0 ? t.Hours / grandTotal * 100 : 0))
            .OrderByDescending(s => s.TotalHours)
            .ThenBy(s => s.Name)
            .ToList();
    }

    private static bool IsValid(TimeEntryDto entry) =>
        !string.IsNullOrWhiteSpace(entry.EmployeeName)
        && entry.DeletedOn is null
        && entry.StartTimeUtc.HasValue
        && entry.EndTimeUtc.HasValue
        && entry.EndTimeUtc.Value > entry.StartTimeUtc.Value;
}
