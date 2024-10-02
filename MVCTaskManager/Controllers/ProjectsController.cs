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
        public ProjectsController(IConfiguration configuration, ILogger<ProjectsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [Route("/")]
        [Route("api/Projects")]
        public async Task<List<Project>> GetProjects()
        {
            try
            {
                _logger.LogInformation("GetProjects Entered");
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
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
            catch(Exception ex) 
            {
                _logger.LogError(ex.Message,"GetProjects Error Message");
                _logger.LogError(ex.StackTrace,"GetProjects Error StackTrace");
                 return new List<Project>();
            }

        }
        // Method to convert DataTable to List<T>
        public  async Task<List<T>> ConvertDataTableToList<T>(DataTable table) where T : new()
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
