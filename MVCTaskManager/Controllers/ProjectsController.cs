using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MVCTaskManager.Models;
using System.Data;

namespace MVCTaskManager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProjectsController> _logger;
        private readonly string connectionString;
        public ProjectsController(IConfiguration configuration, ILogger<ProjectsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            connectionString = _configuration.GetConnectionString("DefaultConnection"); ;
        }

        [HttpGet]
        [Route("/")]
        [Route("api/getProjects")]
        public async Task<List<Project>> GetProjects()
        {
            try
            {
                DataSet dsProjects = new DataSet();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand("GetProjectsList", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(dsProjects);
                    // Convert DataTable to List<User>
                    List<Project> projects = await ConvertDataTableToList<Project>(dsProjects.Tables[0]);
                    return projects;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetProjects Error Message");
                _logger.LogError(ex.StackTrace, "GetProjects Error StackTrace");
                 return new List<Project>();
            }

        }

        [HttpPost]
        [Route("api/insertProject")]
        public async Task<IActionResult> InsertProject([FromBody] Project project)
        {
            try
            {
                if (project.ProjectId <= 0 || string.IsNullOrWhiteSpace(project.ProjectName) || project.TeamSize <= 0 || project.DateOfStart == DateTime.MinValue)
                {
                    return BadRequest("One of the Project parameter is null or empty.");
                }
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync(); 
                    SqlCommand sqlCommand = new SqlCommand("InsertProject", sqlConnection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ProjectId", project.ProjectId));
                    sqlCommand.Parameters.Add(new SqlParameter("@ProjectName", project.ProjectName));
                    sqlCommand.Parameters.Add(new SqlParameter("@DateOfStart", project.DateOfStart));
                    sqlCommand.Parameters.Add(new SqlParameter("@TeamSize", project.TeamSize));
                    await sqlCommand.ExecuteNonQueryAsync();
                }
                 return StatusCode(201,new { message = "Successfully inserted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "InsertProject Error Message");
                _logger.LogError(ex.StackTrace, "InsertProject Error Stacktrace");
                 return StatusCode(500, new { message = "Internal server error while inserting project." });
            }
        }

        [HttpPost]
        [Route("api/updateProject")]
        public async Task<IActionResult> UpdateProject([FromBody] Project project)
        {
            try
            {
                if (project.ProjectId <= 0 || string.IsNullOrWhiteSpace(project.ProjectName) || project.TeamSize <= 0 || project.DateOfStart == DateTime.MinValue)
                {
                    return BadRequest("One of the Project parameter is null or empty.");
                }
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    SqlCommand sqlCommand = new SqlCommand("UpdateProject", sqlConnection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ProjectId", project.ProjectId));
                    sqlCommand.Parameters.Add(new SqlParameter("@ProjectName", project.ProjectName));
                    sqlCommand.Parameters.Add(new SqlParameter("@DateOfStart", project.DateOfStart));
                    sqlCommand.Parameters.Add(new SqlParameter("@TeamSize", project.TeamSize));

                    SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    sqlCommand.Parameters.Add(outputParam);
                    await sqlCommand.ExecuteNonQueryAsync();

                    // Get the value of the output parameter
                    int result = (int)outputParam.Value;
                    if (result == 1)
                    {
                        return StatusCode(200, new { message = "project Updated Successfully." });
                    }
                    else 
                    {
                        return StatusCode(404, new { message = "record not found in db" });
                    }
                }              

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "UpdateProject Error Message");
                _logger.LogError(ex.StackTrace, "UpdateProject Error SatckTrace");
                return StatusCode(500, new { message = "Internal server error while updating project request." });

            }
        }

        [HttpGet]
        [Route("api/deleteProject/{projectId}")]
        public async Task<IActionResult> deleteProject(int projectId)
        {
            try
            {
                if (projectId <= 0)
                {
                    return BadRequest("Invalid Project ID.");
                }
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    SqlCommand sqlCommand = new SqlCommand("deleteProject", sqlConnection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@projectId", projectId));
                    SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int)
                    {
                        Direction=ParameterDirection.Output
                    };
                    sqlCommand.Parameters.Add(outputParam);
                    await sqlCommand.ExecuteNonQueryAsync();

                    //Get the value of output param
                    int result = (int)outputParam.Value;
                    if (result == 1)
                    {
                        return StatusCode(200, new { message = "project Updated Successfully." });
                    }
                    else if (result == 0)
                    {
                        return StatusCode(404, new { message = "no record found in db." });
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Internal server error while updating project request." });
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "deleteProject Error Message");
                _logger.LogError(ex.StackTrace, "deleteProject Error stackTrace");
                return StatusCode(500, new { message = "Internal server error while updating project request." });
            }
        }
        // Method to convert DataTable to List<T>
        public async Task<List<T>> ConvertDataTableToList<T>(DataTable table) where T : new()
        {
            try
            {
                return await Task.Run(() =>
                {
                    List<T> list = new List<T>();

                    foreach (DataRow row in table.Rows)
                    {
                        T obj = new T();

                        // Loop through the properties of the class
                        foreach (var prop in typeof(T).GetProperties())
                        {
                            if (table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                            {
                                // Set the property value from the DataTable row
                                prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType), null);
                            }
                        }

                        list.Add(obj);
                    }

                    return list;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "ConvertDataTableToList Error Message");
                _logger.LogError(ex.StackTrace, "ConvertDataTableToList Error StackTrace");
                return new List<T>();
            }
        }
    }
}
