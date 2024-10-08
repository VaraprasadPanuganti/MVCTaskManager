using MVCTaskManager.Models;
using System.Data;

namespace MVCTaskManager.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        public Task<DataTable> Login(LoginRequest loginRequest);
    }
}
