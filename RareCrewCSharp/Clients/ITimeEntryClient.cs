using RareCrewCSharp.DTO;

namespace RareCrewCSharp.Clients;

public interface ITimeEntryClient
{
    Task<IReadOnlyList<TimeEntryDto>> GetTimeEntriesAsync(CancellationToken cancellationToken = default);
}
