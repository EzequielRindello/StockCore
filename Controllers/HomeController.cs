using Microsoft.AspNetCore.Mvc;

namespace StockCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var data = await _homeService.GetDashboardDataAsync();
            return Json(data);
        }

        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("~/Views/Shared/NotFound.cshtml");
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
}