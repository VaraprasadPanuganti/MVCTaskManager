using MVCTaskManager.Common;
using MVCTaskManager.Models;
using MVCTaskManager.Repositories.Interfaces;
using MVCTaskManager.Services.Interfaces;
using System.Data;

namespace MVCTaskManager.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository clientRepository;
        public ClientService(IClientRepository _clientRepository)
        {
            clientRepository = _clientRepository;
        }

        public async Task<List<ClientLocations>> GetClientLocations()
        {
            DataTable dataTable = await clientRepository.GetClientLocations();
            List<ClientLocations> clientLocations = await CommonHelper.ConvertDataTableToList<ClientLocations>(dataTable);
            return clientLocations;
        }
    }
}
