using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetDashboardData()
    {
        var data = await _homeService.GetDashboardDataAsync();
        return Json(data);
    }

    [HttpGet]
    [RequireActiveUser]
    public async Task<IActionResult> ExportDashboardReport()
    {
        var csv = await _homeService.ExportDashboardReportAsync();

        return File(
            System.Text.Encoding.UTF8.GetBytes(csv),
            "text/csv",
            "dashboard-report.csv"
        );
    }
}
