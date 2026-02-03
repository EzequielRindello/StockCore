namespace StockCore.Services.Interfaces
{
    public interface IUserService
    {
        Task<IndexViewResult> GetIndexView(string? userId);
        Task<List<UserList>> FilterUsers(UserFilter filter);
        Task<UserForm> GetForEdit(string id);
        Task<(string key, string message)> CreateUser(UserForm model);
        Task<(UserForm model, string key, string message)> UpdateUser(UserForm model);
        Task<(string key, string message)> SendPasswordReset(string userId);
        Task<(string key, string message)> ChangePassword(string userId, string newPassword);
        Task<(string key, string message)> DeleteUser(string userId, string currentUserId);
        Task<AuthResult> AuthenticateUser(string email, string password);
    }
}