using RareCrewCSharp.Models;

namespace RareCrewCSharp.ViewModels;

public class EmployeeTableViewModel
{
    public const double LowHoursThreshold = 100;

    public IReadOnlyList<EmployeeSummary> Employees { get; init; } = Array.Empty<EmployeeSummary>();

    public string? ErrorMessage { get; init; }

    public static bool IsBelowThreshold(EmployeeSummary employee) => employee.TotalHours < LowHoursThreshold;
}
