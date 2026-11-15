using RareCrewCSharp.DTO;

namespace RareCrewCSharp.Models;

public class EmployeesViewModel
{
    public const double LowHoursLimit = 100;

    public List<EmployeeDTO> Employees { get; set; }
    public string PieChart { get; set; }
}
