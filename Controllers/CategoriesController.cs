using Microsoft.AspNetCore.Mvc;

namespace StockCore.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
