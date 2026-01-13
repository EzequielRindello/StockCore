using Microsoft.AspNetCore.Mvc;

namespace StockCore.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
