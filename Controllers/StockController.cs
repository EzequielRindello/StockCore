using Microsoft.AspNetCore.Mvc;

namespace StockCore.Controllers
{
    public class StockController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
