using Microsoft.AspNetCore.Mvc;
using StockCore.Services.Const;
using System.Text;

public class StockController : Controller
{
    private readonly IStockService _stockMovementService;
    private readonly IComboService _comboService;

    public StockController(
        IStockService stockMovementService,
        IComboService comboService)
    {
        _stockMovementService = stockMovementService;
        _comboService = comboService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Products = await _comboService.GetProductsCombo();
        return View(await _stockMovementService.GetAllStockMovements());
    }

    [HttpPost]
    public async Task<IActionResult> Filter(StockMovementFilter filter)
    {
        var movements = await _stockMovementService.FilterStockMovements(filter);
        return PartialView("_StockMovementTable", movements);
    }

    public async Task<IActionResult> Details(int id)
    {
        return View(await _stockMovementService.GetDetail(id));
    }

    public async Task<IActionResult> Create()
    {
        var vm = new StockMovementFormView
        {
            Products = await _comboService.GetProductsCombo()
        };
        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> Create(StockMovementFormView vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Products = await _comboService.GetProductsCombo();
            return View(vm);
        }

        TempData.Merge(await _stockMovementService.CreateStockMovement(vm.StockMovement));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vm = new StockMovementFormView
        {
            StockMovement = await _stockMovementService.GetForEdit(id),
            Products = await _comboService.GetProductsCombo()
        };
        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> Edit(StockMovementFormView vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Products = await _comboService.GetProductsCombo();
            return View(vm);
        }

        var result = await _stockMovementService.UpdateStockMovement(vm.StockMovement);
        if (result.Item2 == ValidationMessages.ERROR)
        {
            vm.StockMovement = result.Item1;
            vm.Products = await _comboService.GetProductsCombo();
            ViewBag.MessageKey = result.Item2;
            ViewBag.Message = result.Item3;
            return View(vm);
        }

        vm.Products = await _comboService.GetProductsCombo();
        ViewBag.MessageKey = result.Item2;
        ViewBag.Message = result.Item3;
        return View(vm);
    }

    [HttpPost]
    [RequireActiveUser]
    public async Task<IActionResult> DeleteMany(List<int> ids)
    {
        TempData.Merge(await _stockMovementService.DeleteManyStockMovements(ids));
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequireActiveUser]
    public async Task<IActionResult> ExportCsv([FromQuery] StockMovementFilter filter)
    {
        var csv = await _stockMovementService.ExportStockMovementsCsvAsync(filter);

        return File(
            Encoding.UTF8.GetBytes(csv),
            "text/csv",
            "stock-movements.csv"
        );
    }
}