using MVCTaskManager.Models;
using System.Data;

namespace MVCTaskManager.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        public Task<DataTable> Login(LoginRequest loginRequest);

        public Task<DataTable> FindUserbyEmail(PasswordResetRequest passwordResetRequest);

        public Task<DataTable> GetUserById(string UserId);
        Task<int> UpdateUserPassword(string userId, string password);
    }
}
