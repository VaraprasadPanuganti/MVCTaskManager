using MVCTaskManager.Models;

namespace MVCTaskManager.Services.Interfaces
{
    public interface IClientService
    {
        public Task<List<ClientLocations>> GetClientLocations();
    }
}
