using Microsoft.AspNetCore.Mvc;
using StockCore.Services.Interfaces;

public class ProductsController : Controller
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _service.GetAllProducts());
    }

    public async Task<IActionResult> Details(int id)
    {
        return View(await _service.GetDetail(id));
    }

    public async Task<IActionResult> Edit(int id)
    {
        return View(await _service.GetForEdit(id));
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(ProductForm model)
    {
        TempData.Merge(await _service.CreateProduct(model));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductForm model)
    {
        var result = await _service.UpdateProduct(model);
        TempData.Merge((result.key, result.message));

        return View(result.model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMany(List<int> ids)
    {
        TempData.Merge(await _service.DeleteManyProducts(ids));
        return RedirectToAction(nameof(Index));
    }
}
