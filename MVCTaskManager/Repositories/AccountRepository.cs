using Microsoft.Data.SqlClient;
using MVCTaskManager.Models;
using MVCTaskManager.Repositories.Interfaces;
using MVCTaskManager.SHA256Gen;
using System.Data;

namespace MVCTaskManager.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        public AccountRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<DataTable> Login(LoginRequest loginRequest)
        {
            DataTable userDetails = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string passwordhash = SHA256HashGenerator.GenerateHash(loginRequest?.Password);
                SqlCommand sqlCommand = new SqlCommand("GetUserDetailsByEmailAndPassword", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Email", loginRequest?.Email));
                sqlCommand.Parameters.Add(new SqlParameter("@Password", passwordhash));
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(userDetails);
            }
            return userDetails;
        }
    }
}
