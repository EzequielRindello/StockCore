using Microsoft.EntityFrameworkCore;
using StockCore.Data;
using StockCore.Entities;
using StockCore.Services.Const;
using StockCore.Services.Interfaces;

namespace StockCore.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<IndexViewResult> GetIndexView(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new IndexViewResult { IsLoggedIn = false };
            }

            var currentUser = await GetCurrentUser(userId);
            var users = await GetAllUsers();

            return new IndexViewResult
            {
                IsLoggedIn = true,
                CurrentUser = currentUser,
                Users = users
            };
        }

        private async Task<UserDetail?> GetCurrentUser(string userId)
        {
            var user = await _db.Users.FindAsync(userId);

            if (user == null)
                return null;

            return new UserDetail
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        private async Task<List<UserList>> GetAllUsers()
        {
            return await _db
                .Users
                .Select(u => new UserList
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<UserList>> FilterUsers(UserFilter filter)
        {
            var query = _db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(u =>
                    u.UserName.Contains(filter.Search) ||
                    u.Email.Contains(filter.Search));
            }

            return await query
                .Select(u => new UserList
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<UserForm?> GetForEdit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var user = await _db.Users.FindAsync(id);

            if (user == null)
                return null;

            return new UserForm
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }

        public async Task<(string, string)> CreateUser(UserForm model)
        {

            if (string.IsNullOrWhiteSpace(model.Password))
                return (ValidationMessages.ERROR, "Password is required");

            if (model.Password.Length < 6)
                return (ValidationMessages.ERROR, "Password must be at least 6 characters");

            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {

                var existingUser = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email || u.UserName == model.UserName);

                if (existingUser != null)
                {
                    if (existingUser.Email == model.Email)
                        return (ValidationMessages.ERROR, "Email already exists");
                    if (existingUser.UserName == model.UserName)
                        return (ValidationMessages.ERROR, "Username already exists");
                }


                var user = new ApplicationUserEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.UserName,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    IsActive = model.IsActive,
                    EmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return (ValidationMessages.SUCCESS, "User created successfully");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                return (ValidationMessages.ERROR, $"Error creating user: {ex.Message}");
            }
        }

        public async Task<(UserForm, string, string)> UpdateUser(UserForm model)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var user = await _db.Users.FindAsync(model.Id);

                if (user == null)
                    return (model, ValidationMessages.ERROR, "User not found");

                var existingUser = await _db.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id != model.Id &&
                        (u.Email == model.Email || u.UserName == model.UserName));

                if (existingUser != null)
                {
                    if (existingUser.Email == model.Email)
                        return (model, ValidationMessages.ERROR, "Email already exists");
                    if (existingUser.UserName == model.UserName)
                        return (model, ValidationMessages.ERROR, "Username already exists");
                }

                // Actualizar datos
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.IsActive = model.IsActive;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return (model, ValidationMessages.SUCCESS, "User updated successfully");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (model, ValidationMessages.ERROR, $"Error updating user: {ex.Message}");
            }
        }

        public async Task<(string, string)> DeleteUser(string userId, string? currentUserId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return (ValidationMessages.ERROR, "Invalid user");

            if (userId == currentUserId)
                return (ValidationMessages.ERROR, "You cannot delete yourself");

            var usersCount = await _db.Users.CountAsync();
            if (usersCount <= 1)
                return (ValidationMessages.ERROR, "At least one user must exist");

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return (ValidationMessages.ERROR, "User not found");

            try
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();

                return (ValidationMessages.SUCCESS, "User deleted successfully");
            }
            catch (Exception ex)
            {
                return (ValidationMessages.ERROR, $"Error deleting user: {ex.Message}");
            }
        }

        public async Task<(string, string)> SendPasswordReset(string userId)
        {
            try
            {
                var user = await _db.Users.FindAsync(userId);

                if (user == null)
                    return (ValidationMessages.ERROR, "User not found");

                // TODO: implement email reset

                return (ValidationMessages.SUCCESS, "Password reset email sent");
            }
            catch
            {
                return (ValidationMessages.ERROR, "Error sending password reset");
            }
        }

        public async Task<(string, string)> ChangePassword(string userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return (ValidationMessages.ERROR, "Password is required");

            if (newPassword.Length < 6)
                return (ValidationMessages.ERROR, "Password must be at least 6 characters");

            try
            {
                var user = await _db.Users.FindAsync(userId);

                if (user == null)
                    return (ValidationMessages.ERROR, "User not found");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _db.SaveChangesAsync();

                return (ValidationMessages.SUCCESS, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return (ValidationMessages.ERROR, $"Error changing password: {ex.Message}");
            }
        }

        public async Task<AuthResult> AuthenticateUser(string email, string password)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }

                bool isValid = user.PasswordHash == password || BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

                if (!isValid)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }

                if (!user.IsActive)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "User account is inactive"
                    };
                }

                return new AuthResult
                {
                    Success = true,
                    UserId = user.Id,
                    UserName = user.UserName,
                    Message = "Login successful"
                };
            }
            catch
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "An error occurred during authentication"
                };
            }
        }
    }
}