using RareCrewCSharp.DTO;

namespace RareCrewCSharp.Queries;

public interface IGetEmployeesQuery : IQuery<EmptyQueryParams, List<EmployeeDTO>>
{
}
