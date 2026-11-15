using RareCrewCSharp.DTO;
using RareCrewCSharp.Queries;

namespace RareCrewCSharp.Implementation.Queries;

public class ApiGetEmployeesQuery : IGetEmployeesQuery
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _settings;

    public ApiGetEmployeesQuery(HttpClient httpClient, AppSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<List<EmployeeDTO>> Execute(EmptyQueryParams param)
    {
        var api = _settings.ApiSettings;
        var url = $"{api.Url}/{api.Endpoint}?code={api.Key}";

        var timeEntries = await _httpClient.GetFromJsonAsync<List<TimeEntryDTO>>(url) ?? new List<TimeEntryDTO>();

        var employees = timeEntries
            .Where(x => !string.IsNullOrWhiteSpace(x.EmployeeName)
                        && x.DeletedOn == null
                        && x.EndTimeUtc > x.StarTimeUtc)
            .GroupBy(x => x.EmployeeName.Trim())
            .Select(g => new EmployeeDTO
            {
                Name = g.Key,
                TotalHours = g.Sum(x => (x.EndTimeUtc - x.StarTimeUtc).TotalHours),
            })
            .OrderByDescending(x => x.TotalHours)
            .ToList();

        var totalHours = employees.Sum(x => x.TotalHours);

        foreach (var employee in employees)
        {
            employee.Percentage = totalHours > 0 ? employee.TotalHours / totalHours * 100 : 0;
        }

        return employees;
    }
}
