using Microsoft.Data.SqlClient;
using MVCTaskManager.Models;
using MVCTaskManager.Repositories.Interfaces;
using System.Data;

namespace MVCTaskManager.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;
        public ProjectRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> DeleteProject(int projectId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand("deleteProject", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@projectId", projectId));
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

        public async Task<DataTable> GetAllProjects()
        {
           DataTable dtProjects = new DataTable();
           using (SqlConnection sqlConnection = new SqlConnection(connectionString))
           {
              await sqlConnection.OpenAsync();
              SqlCommand sqlCommand = new SqlCommand("GetProjectsList", sqlConnection);
              sqlCommand.CommandType = CommandType.StoredProcedure;
              SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
              sqlDataAdapter.Fill(dtProjects);
           }
            return dtProjects;
        }

        public async Task AddProject(Project project)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand("InsertProject", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ProjectId", project.ProjectId));
                sqlCommand.Parameters.Add(new SqlParameter("@ProjectName", project.ProjectName));
                sqlCommand.Parameters.Add(new SqlParameter("@DateOfStart", project.DateOfStart));
                sqlCommand.Parameters.Add(new SqlParameter("@TeamSize", project.TeamSize));
                sqlCommand.Parameters.Add(new SqlParameter("@Active", project.Active));
                sqlCommand.Parameters.Add(new SqlParameter("@Status", project.Status));
                sqlCommand.Parameters.Add(new SqlParameter("@ClientLocationId", project.ClientLocation));
                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<DataTable> SearchProject(ProjectSearchRequest projectSearchRequest)
        {
            DataTable ds = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand("searchProjectsWithColumn", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ColumnName", projectSearchRequest.ColumnName));
                sqlCommand.Parameters.Add(new SqlParameter("@SearchValue", projectSearchRequest.SearchParameter));
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(ds);
                return ds;
            }
        }

        public async Task<int> UpdateProject(Project project)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand("UpdateProject", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ProjectId", project.ProjectId));
                sqlCommand.Parameters.Add(new SqlParameter("@ProjectName", project.ProjectName));
                sqlCommand.Parameters.Add(new SqlParameter("@DateOfStart", project.DateOfStart));
                sqlCommand.Parameters.Add(new SqlParameter("@TeamSize", project.TeamSize));
                sqlCommand.Parameters.Add(new SqlParameter("@Active", project.Active));
                sqlCommand.Parameters.Add(new SqlParameter("@Status", project.Status));
                sqlCommand.Parameters.Add(new SqlParameter("@ClientLocationId", project.ClientLocationId));

                SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                sqlCommand.Parameters.Add(outputParam);
                await sqlCommand.ExecuteNonQueryAsync();

                // Get the value of the output parameter
                int result = (int)outputParam.Value;
                return result;
            }
        }
    }
}
