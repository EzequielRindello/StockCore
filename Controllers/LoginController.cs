using Microsoft.AspNetCore.Mvc;

namespace StockCore.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
