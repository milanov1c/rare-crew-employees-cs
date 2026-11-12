using RareCrewCSharp.Models;

namespace RareCrewCSharp.Services;

public interface IEmployeeReportService
{
    Task<IReadOnlyList<EmployeeSummary>> GetEmployeeSummariesAsync(CancellationToken cancellationToken = default);
}
