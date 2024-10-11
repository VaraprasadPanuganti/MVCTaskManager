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

        public async Task<DataTable> FindUserbyEmail(PasswordResetRequest passwordResetRequest)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand("FindUsersByEmail", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Email", passwordResetRequest.EmailId));
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataTable);
               
            }
            return dataTable;
        }

        public async Task<DataTable> GetUserById(string UserId)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand($"select * from users where UserID='{UserId}'", sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
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

        public async Task<int> UpdateUserPassword(string userId, string password)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand("UpdateUserPassword", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@UserId", userId));
                sqlCommand.Parameters.Add(new SqlParameter("@Password", password));
                SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(outputParam);
                await sqlCommand.ExecuteNonQueryAsync();

                //Get the value of output param
                int result = (int)outputParam.Value;
                return result;
            }
        }
    }
}
