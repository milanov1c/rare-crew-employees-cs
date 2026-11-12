using RareCrewCSharp.Models;

namespace RareCrewCSharp.Charts;

public interface IChartGenerator
{
    byte[] GeneratePieChartPng(IReadOnlyList<EmployeeSummary> employees);
}
