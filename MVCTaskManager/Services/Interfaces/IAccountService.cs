using MVCTaskManager.Models;

namespace MVCTaskManager.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<List<LoginResponse>> Login(LoginRequest loginRequest);
    }
}
