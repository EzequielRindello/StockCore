public class UserFormView
{
    public UserForm User { get; set; } = new();
    public ChangePasswordForm ChangePassword { get; set; } = new();

    public bool IsEdit { get; set; }
}
