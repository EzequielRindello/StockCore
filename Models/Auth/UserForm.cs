using StockCore.Services.Const;
using System.ComponentModel.DataAnnotations;

public class UserForm
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.UserNameRequired)]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.EmailRequired)]
    [MaxLength(150)]
    [EmailAddress(ErrorMessage = ValidationMessages.InvalidEmail)]
    public string Email { get; set; } = string.Empty;

    [MinLength(6, ErrorMessage = ValidationMessages.InvalidPassword)]
    public string? Password { get; set; }

    public bool IsActive { get; set; } = true;
}
