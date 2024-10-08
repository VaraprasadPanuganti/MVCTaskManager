using Microsoft.Data.SqlClient;
using MVCTaskManager.Repositories.Interfaces;
using MVCTaskManager.Services.Interfaces;
using System.Data;

namespace MVCTaskManager.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public ClientRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<DataTable> GetClientLocations()
        {
            DataTable dsClientLocations = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand("GetClientLocations", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dsClientLocations);
            }
            return dsClientLocations;
         }
    }
}
