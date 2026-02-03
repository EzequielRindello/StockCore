using Microsoft.AspNetCore.Mvc;
using StockCore.Services.Const;
using StockCore.Services.Interfaces;
using System.Text;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly IComboService _comboService;

    public ProductsController(
        IProductService productService,
        IComboService comboService)
    {
        _productService = productService;
        _comboService = comboService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Categories = await _comboService.GetCategoriesCombo();
        return View(await _productService.GetAllProducts());
    }

    [HttpPost]
    public async Task<IActionResult> Filter(ProductFilter filter)
    {
        var products = await _productService.FilterProducts(filter);
        return PartialView("_ProductTable", products);
    }

    public async Task<IActionResult> Details(int id)
    {
        return View(await _productService.GetDetail(id));
    }

    public async Task<IActionResult> Create()
    {
        var vm = new ProductFormView
        {
            Categories = await _comboService.GetCategoriesCombo()
        };

        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> Create(ProductFormView vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await _comboService.GetCategoriesCombo();
            return View(vm);
        }

        TempData.Merge(await _productService.CreateProduct(vm.Product));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vm = new ProductFormView
        {
            Product = await _productService.GetForEdit(id),
            Categories = await _comboService.GetCategoriesCombo()
        };

        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> Edit(ProductFormView vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await _comboService.GetCategoriesCombo();
            return View(vm);
        }

        var result = await _productService.UpdateProduct(vm.Product);

        if (result.key == ValidationMessages.ERROR)
        {
            vm.Product = result.model;
            vm.Categories = await _comboService.GetCategoriesCombo();
            ViewBag.MessageKey = result.key;
            ViewBag.Message = result.message;
            return View(vm);
        }

        vm.Categories = await _comboService.GetCategoriesCombo();
        ViewBag.MessageKey = result.key;
        ViewBag.Message = result.message;

        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> DeleteMany(List<int> ids)
    {
        TempData.Merge(await _productService.DeleteManyProducts(ids));
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequireActiveUser]
    public async Task<IActionResult> ExportCsv([FromQuery] ProductFilter filter)
    {
        var csv = await _productService.ExportProductsCsvAsync(filter);

        return File(
            Encoding.UTF8.GetBytes(csv),
            "text/csv",
            "products.csv"
        );
    }
}
