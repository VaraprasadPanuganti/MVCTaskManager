using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MVCTaskManager.Common;
using MVCTaskManager.Models;
using System.Data;

namespace MVCTaskManager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientController> _logger;
        private readonly string connectyionString; 
        public ClientController(IConfiguration configuration, ILogger<ClientController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            connectyionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("getClientLocations")]
        public async Task<ActionResult> GetClientLocations()
        {
            try
            {
                DataSet dsClientLocations = new DataSet();
                using (SqlConnection sqlConnection = new SqlConnection(connectyionString))
                {
                   await sqlConnection.OpenAsync();
                   SqlCommand sqlCommand = new SqlCommand("GetClientLocations", sqlConnection);
                   sqlCommand.CommandType = CommandType.StoredProcedure;
                   SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                   sqlDataAdapter.Fill(dsClientLocations);
                   List<ClientLocations> clientLocations = await CommonHelper.ConvertDataTableToList<ClientLocations>(dsClientLocations.Tables[0]);
                   return StatusCode(200, new { message ="retriving successfull",data= clientLocations});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetClientLocations Error Message");
                _logger.LogError(ex.Message, "GetClientLocations Error StackTrace");
                return StatusCode(500, new { message = "retriving getClientLocations failed" });
            }
        }
    }
}
