using MVCTaskManager.Models;

namespace MVCTaskManager.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<List<LoginResponse>> Login(LoginRequest loginRequest);
        public Task<LoginResponse> FindUserbyEmail(PasswordResetRequest passwordResetReques);
        public Task<LoginResponse> GetUserById(string UserId);
        public Task<int> UpdateUserPassword(string userId, string password);
    }
}
