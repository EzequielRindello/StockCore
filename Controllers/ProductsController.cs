using Microsoft.AspNetCore.Mvc;
using StockCore.Services.Interfaces;

namespace StockCore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _service.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _service.GetDetailAsync(id);
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.GetForEditAsync(id);
            return View(model);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ProductForm model)
        {
            if (!ModelState.IsValid) return View(model);

            await _service.CreateAsync(model);
            TempData["Success"] = "Product created successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductForm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.UpdateAsync(model);
            TempData["Success"] = "Product saved successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMany(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                TempData["Error"] = "No products selected";
                return RedirectToAction(nameof(Index));
            }

            await _service.DeleteManyAsync(ids);
            TempData["Success"] = "Products deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
