using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RareCrewCSharp.DTO;
using RareCrewCSharp.Models;
using RareCrewCSharp.Queries;

namespace RareCrewCSharp.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index(
        [FromServices] IGetEmployeesQuery query,
        [FromServices] IChartGenerator chartGenerator)
    {
        var employees = await query.Execute(new EmptyQueryParams());
        var pieChart = chartGenerator.GeneratePieChart(employees);

        return View(new EmployeesViewModel
        {
            Employees = employees,
            PieChart = Convert.ToBase64String(pieChart),
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
