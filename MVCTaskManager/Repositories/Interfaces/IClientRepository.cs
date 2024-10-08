using System.Data;

namespace MVCTaskManager.Repositories.Interfaces
{
    public interface IClientRepository
    {
        public Task<DataTable> GetClientLocations();
    }
}
