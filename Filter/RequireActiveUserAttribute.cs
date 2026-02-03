using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using StockCore.Services.Interfaces;

public class RequireActiveUserAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var http = context.HttpContext;
        var userId = http.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
        {
            Deny(context, "You must be logged in.");
            return;
        }

        var userService = http.RequestServices.GetRequiredService<IUserService>();

        if (!await userService.IsUserActive(userId))
        {
            http.Session.Clear();
            Deny(context, "Your account is inactive.");
            return;
        }

        await next();
    }

    private static void Deny(ActionExecutingContext context, string message)
    {
        var tempDataFactory =
            context.HttpContext.RequestServices
                .GetRequiredService<ITempDataDictionaryFactory>();

        var tempData = tempDataFactory.GetTempData(context.HttpContext);
        tempData["Error"] = message;

        context.Result = new RedirectToActionResult("Index", "Login", null);
    }
}
