using Microsoft.AspNetCore.Mvc;
using StockCore.Services.Const;
using StockCore.Services.Interfaces;

public class LoginController : Controller
{
    private readonly IUserService _service;

    public LoginController(IUserService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var result = await _service.GetIndexView(userId);

        if (!result.IsLoggedIn)
            return View("Login");

        ViewBag.CurrentUser = result.CurrentUser;
        return View(result.Users);
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _service.AuthenticateUser(email, password);

        if (!result.Success)
        {
            ViewBag.ErrorMessage = result.Message;
            ViewBag.Email = email;
            return View("Login");
        }

        HttpContext.Session.SetString("UserId", result.UserId);
        HttpContext.Session.SetString("UserName", result.UserName);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Create()
    {
        var model = new UserFormView
        {
            IsEdit = false,
            User = new UserForm()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserFormView model)
    {

        if (!ModelState.IsValid)
        {
            model.IsEdit = false;
            return View(model);
        }

        var (key, message) = await _service.CreateUser(model.User);

        if (key == ValidationMessages.ERROR)
        {
            ViewBag.ErrorMessage = message;
            model.IsEdit = false;
            return View(model);
        }

        TempData["MessageKey"] = key;
        TempData["Message"] = message;
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _service.GetForEdit(id);
        if (user == null)
        {
            TempData["MessageKey"] = ValidationMessages.ERROR;
            TempData["Message"] = "User not found";
            return RedirectToAction("Index");
        }

        var model = new UserFormView
        {
            User = user,
            ChangePassword = new ChangePasswordForm { UserId = user.Id },
            IsEdit = true
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserFormView model)
    {

        ModelState.Remove("User.Password");
        ModelState.Remove("ChangePassword.NewPassword");
        ModelState.Remove("ChangePassword.ConfirmPassword");

        if (!ModelState.IsValid)
        {
            model.IsEdit = true;
            return View(model);
        }

        var (updatedModel, key, message) = await _service.UpdateUser(model.User);

        if (key == ValidationMessages.ERROR)
        {
            ViewBag.ErrorMessage = message;
            model.IsEdit = true;
            return View(model);
        }

        TempData["MessageKey"] = key;
        TempData["Message"] = message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordForm model)
    {

        if (string.IsNullOrWhiteSpace(model?.UserId))
        {
            TempData["MessageKey"] = ValidationMessages.ERROR;
            TempData["Message"] = "Invalid user ID";
            return RedirectToAction("Index");
        }

        var keysToRemove = ModelState.Keys
            .Where(k => k.StartsWith("User.") || k.StartsWith("ChangePassword."))
            .ToList();

        foreach (var keys in keysToRemove)
        {
            ModelState.Remove(keys);
        }

        if (!ModelState.IsValid)
        {
            var user = await _service.GetForEdit(model.UserId);
            if (user == null)
            {
                TempData["MessageKey"] = ValidationMessages.ERROR;
                TempData["Message"] = "User not found";
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Please fill all password fields correctly";
            var viewModel = new UserFormView
            {
                User = user,
                ChangePassword = model,
                IsEdit = true
            };
            return View("Edit", viewModel);
        }

        var (key, message) = await _service.ChangePassword(model.UserId, model.NewPassword);

        if (key == ValidationMessages.ERROR)
        {
            var user = await _service.GetForEdit(model.UserId);
            if (user == null)
            {
                TempData["MessageKey"] = ValidationMessages.ERROR;
                TempData["Message"] = "User not found";
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = message;
            var viewModel = new UserFormView
            {
                User = user,
                ChangePassword = model,
                IsEdit = true
            };
            return View("Edit", viewModel);
        }

        TempData["MessageKey"] = key;
        TempData["Message"] = message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUserId = HttpContext.Session.GetString("UserId");
        var (key, message) = await _service.DeleteUser(id, currentUserId);

        TempData["MessageKey"] = key;
        TempData["Message"] = message;

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Filter(UserFilter filter)
    {
        var users = await _service.FilterUsers(filter);
        return PartialView("_UserTable", users);
    }

    [HttpPost]
    public async Task<IActionResult> SendPasswordReset(string userId)
    {
        var (key, message) = await _service.SendPasswordReset(userId);
        return Json(new { key, message });
    }
}