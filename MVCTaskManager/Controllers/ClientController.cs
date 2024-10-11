using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCTaskManager.Models;
using MVCTaskManager.Services.Interfaces;

namespace MVCTaskManager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
    
        private readonly ILogger<ClientController> _logger; 
        private readonly IClientService clientService;
        public ClientController(ILogger<ClientController> logger, IClientService _clientService)
        {
            _logger = logger;
            clientService = _clientService;
  
        }

        [HttpGet("getClientLocations")]
        public async Task<ActionResult> GetClientLocations()
        {
            try
            {
                List<ClientLocations> clientLocations = await clientService.GetClientLocations();
                return StatusCode(200, new { message ="retriving successfull",data= clientLocations});
                
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
