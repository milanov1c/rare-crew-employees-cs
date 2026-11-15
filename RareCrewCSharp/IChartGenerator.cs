using RareCrewCSharp.DTO;

namespace RareCrewCSharp;

public interface IChartGenerator
{
    public byte[] GeneratePieChart(List<EmployeeDTO> employees);
}
