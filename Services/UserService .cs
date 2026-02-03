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
                return (ValidationMessages.ERROR, ValidationMessages.PasswordRequiered);

            if (model.Password.Length < 6)
                return (ValidationMessages.ERROR, ValidationMessages.InvalidPassword);

            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var existingUser = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email || u.UserName == model.UserName);

                if (existingUser != null)
                    return (ValidationMessages.ERROR, ValidationMessages.UniqueEmail());

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

                return (ValidationMessages.SUCCESS, ValidationMessages.CreatedMessage("User"));
            }
            catch
            {
                await tx.RollbackAsync();
                return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("creating", "user"));
            }
        }

        public async Task<(UserForm, string, string)> UpdateUser(UserForm model)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var user = await _db.Users.FindAsync(model.Id);
                if (user == null)
                    return (model, ValidationMessages.ERROR, ValidationMessages.NotFound("User"));

                var existingUser = await _db.Users.FirstOrDefaultAsync(u =>
                    u.Id != model.Id &&
                    (u.Email == model.Email || u.UserName == model.UserName));

                if (existingUser != null)
                    return (model, ValidationMessages.ERROR, ValidationMessages.UniqueEmail());

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.IsActive = model.IsActive;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return (model, ValidationMessages.SUCCESS, ValidationMessages.SavedMessage("User"));
            }
            catch
            {
                await tx.RollbackAsync();
                return (model, ValidationMessages.ERROR, ValidationMessages.ErrorMessage("updating", "user"));
            }
        }

        public async Task<(string, string)> DeleteUser(string userId, string? currentUserId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return (ValidationMessages.ERROR, ValidationMessages.SelectedMessage("user"));

            if (userId == currentUserId)
                return (ValidationMessages.ERROR, "You cannot delete yourself");

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return (ValidationMessages.ERROR, ValidationMessages.NotFound("User"));

            var usersCount = await _db.Users.CountAsync();
            if (usersCount <= 1)
                return (ValidationMessages.ERROR, "At least one user must exist");

            var activeUsersCount = await _db.Users.CountAsync(u => u.IsActive && u.Id != userId);
            if (activeUsersCount < 1)
                return (ValidationMessages.ERROR, "At least one active user must exist");

            try
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return (ValidationMessages.SUCCESS, ValidationMessages.DeletedMessage("User"));
            }
            catch
            {
                return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("deleting", "user"));
            }
        }

        public async Task<(string, string)> SendPasswordReset(string userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return (ValidationMessages.ERROR, ValidationMessages.NotFound("User"));

            // TODO: email reset
            return (ValidationMessages.SUCCESS, ValidationMessages.ResetPassword());
        }

        public async Task<(string, string)> ChangePassword(string userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return (ValidationMessages.ERROR, ValidationMessages.PasswordRequiered);

            if (newPassword.Length < 6)
                return (ValidationMessages.ERROR, ValidationMessages.InvalidPassword);

            try
            {
                var user = await _db.Users.FindAsync(userId);
                if (user == null)
                    return (ValidationMessages.ERROR, ValidationMessages.NotFound("User"));

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _db.SaveChangesAsync();

                return (ValidationMessages.SUCCESS, ValidationMessages.SavedMessage("Password"));
            }
            catch
            {
                return (ValidationMessages.ERROR, ValidationMessages.ErrorMessage("changing", "password"));
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

        public async Task<bool> IsUserActive(string userId)
        {
            return await _db
                .Users
                .Where(u => u.Id == userId)
                .Select(u => u.IsActive)
                .FirstOrDefaultAsync();
        }
    }
}