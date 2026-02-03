using Microsoft.AspNetCore.Mvc;
using StockCore.Services.Const;
using System.Text;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _categoryService.GetAllCategories());
    }

    [HttpPost]
    public async Task<IActionResult> Filter(CategoryFilter filter)
    {
        var categories = await _categoryService.FilterCategories(filter);
        return PartialView("_CategoryTable", categories);
    }

    public async Task<IActionResult> Details(int id)
    {
        return View(await _categoryService.GetDetail(id));
    }

    public IActionResult Create()
    {
        var vm = new CategoryFormView();
        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> Create(CategoryFormView vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        TempData.Merge(await _categoryService.CreateCategory(vm.Category));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vm = new CategoryFormView
        {
            Category = await _categoryService.GetForEdit(id)
        };
        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> Edit(CategoryFormView vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var result = await _categoryService.UpdateCategory(vm.Category);
        if (result.Item2 == ValidationMessages.ERROR)
        {
            vm.Category = result.Item1;
            ViewBag.MessageKey = result.Item2;
            ViewBag.Message = result.Item3;
            return View(vm);
        }

        ViewBag.MessageKey = result.Item2;
        ViewBag.Message = result.Item3;
        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> DeleteMany(List<int> ids)
    {
        TempData.Merge(await _categoryService.DeleteManyCategories(ids));
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv([FromQuery] CategoryFilter filter)
    {
        var csv = await _categoryService.ExportCategoriesCsvAsync(filter);

        return File(
            Encoding.UTF8.GetBytes(csv),
            "text/csv",
            "categories.csv"
        );
    }
}