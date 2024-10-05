using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MVCTaskManager.Common;
using MVCTaskManager.Models;
using System.Data;

namespace MVCTaskManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("/")]
        [HttpGet("getProjects")]
        public async Task<IActionResult> GetProjects()
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
                    List<Project> projects = await CommonHelper.ConvertDataTableToList<Project>(dsProjects.Tables[0]);
                    return StatusCode(200, new { message = "suucessfully completed", data = projects });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetProjects Error Message");
                _logger.LogError(ex.StackTrace, "GetProjects Error StackTrace");
                return StatusCode(500, new { message = "Internal server error while inserting project." });
            }

        }

        [HttpPost("insertProject")]
        public async Task<IActionResult> InsertProject([FromBody] Project project)
        {
            try
            {
                if (project == null || project.ProjectId <= 0 || string.IsNullOrWhiteSpace(project.ProjectName) || project.TeamSize <= 0 || project.DateOfStart == DateTime.MinValue)
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
                    sqlCommand.Parameters.Add(new SqlParameter("@Active", project.Active));
                    sqlCommand.Parameters.Add(new SqlParameter("@Status", project.Status));
                    sqlCommand.Parameters.Add(new SqlParameter("@ClientLocationId", project.ClientLocation));
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

        [HttpPost("updateProject")]
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

        [HttpGet("deleteProject/{projectId}")]
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

        [HttpPost("searchProjects")] 
        public async Task<IActionResult> searchProjects([FromBody] ProjectSearchRequest projectSearchRequest)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                { 
                  await sqlConnection.OpenAsync();
                  SqlCommand sqlCommand = new SqlCommand("searchProjectsWithColumn", sqlConnection);
                  sqlCommand.CommandType = CommandType.StoredProcedure;
                  sqlCommand.Parameters.Add(new SqlParameter("@ColumnName", projectSearchRequest.ColumnName));
                  sqlCommand.Parameters.Add(new SqlParameter("@SearchValue", projectSearchRequest.SearchParameter));
                  SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                  sqlDataAdapter.Fill(ds);
                  List<Project> projects = await CommonHelper.ConvertDataTableToList<Project>(ds.Tables[0]);
                  return StatusCode(200, new { message = "suucessfully completed", data= projects });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "searchProjects Error Message");
                _logger.LogError(ex.StackTrace, "searchProjects Error StackTrace");
                 return StatusCode(500, new { message = "Internal server error while searching project request." ,data = new List<Project>() });
            }
        }
        
    }
}
