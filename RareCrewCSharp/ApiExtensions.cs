using RareCrewCSharp.Implementation.Charts;
using RareCrewCSharp.Implementation.Queries;
using RareCrewCSharp.Queries;

namespace RareCrewCSharp;

public static class ApiExtensions
{
    public static void SetupApplication(this IServiceCollection services, AppSettings settings)
    {
        services.AddSingleton(settings);
        services.AddHttpClient<IGetEmployeesQuery, ApiGetEmployeesQuery>();
        services.AddTransient<IChartGenerator, PieChartGenerator>();
    }
}
