using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RareCrewCSharp.Charts;
using RareCrewCSharp.Models;
using RareCrewCSharp.Services;
using RareCrewCSharp.ViewModels;

namespace RareCrewCSharp.Controllers;

public class HomeController : Controller
{
    private readonly IEmployeeReportService _reportService;
    private readonly IChartGenerator _chartGenerator;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IEmployeeReportService reportService,
        IChartGenerator chartGenerator,
        ILogger<HomeController> logger)
    {
        _reportService = reportService;
        _chartGenerator = chartGenerator;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var employees = await _reportService.GetEmployeeSummariesAsync(cancellationToken);
            return View(new EmployeeTableViewModel { Employees = employees });
        }
        catch (Exception ex) when (IsDataSourceError(ex))
        {
            _logger.LogError(ex, "Failed to load employee time entries");
            return View(new EmployeeTableViewModel
            {
                ErrorMessage = "Employee data is currently unavailable. Please try again later."
            });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Chart(bool download = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var employees = await _reportService.GetEmployeeSummariesAsync(cancellationToken);
            var png = _chartGenerator.GeneratePieChartPng(employees);

            return download
                ? File(png, "image/png", "EmployeeChart.png")
                : File(png, "image/png");
        }
        catch (Exception ex) when (IsDataSourceError(ex))
        {
            _logger.LogError(ex, "Failed to generate employee chart");
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static bool IsDataSourceError(Exception ex) =>
        ex is HttpRequestException or JsonException or TaskCanceledException;
}
