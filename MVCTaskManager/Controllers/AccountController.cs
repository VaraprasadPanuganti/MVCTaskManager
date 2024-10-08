using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using MVCTaskManager.Common;
using MVCTaskManager.Models;
using MVCTaskManager.Services.Interfaces;
using MVCTaskManager.SHA256Gen;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVCTaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly IAccountService accountService;

        public AccountController(ILogger<ProjectsController> logger, IAccountService _accountService)
        {
            _logger = logger;
            accountService = _accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                List<LoginResponse> loginResponse = await accountService.Login(loginRequest);
                if (loginResponse != null && loginResponse.Count > 0)
                {
                    return StatusCode(200, new { message = "login Successful", data = loginResponse });
                }
                else
                {
                    return StatusCode(400, new { message = "no user found", data = new List<LoginResponse>() });
                }
    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Login Error Message");
                _logger.LogError(ex.StackTrace, "Login Error StackTrace");
                return StatusCode(500, new { message = "login failed", data = new List<LoginResponse>() });
            }
        }

        //[Authorize]
        //[HttpPost("addUser")]
        //public async Task<IActionResult> addUser([FromBody] AddUserRequest addUserRequest)
        //{
        //    try
        //    {
        //        if (addUserRequest != null && string.IsNullOrEmpty(addUserRequest.Email) && string.IsNullOrEmpty(addUserRequest.UserName) && string.IsNullOrEmpty(addUserRequest.Role) && string.IsNullOrEmpty(addUserRequest.password))
        //        {
        //            return StatusCode(500, new { message = "request parameter can't be null or empty" });
        //        }
        //        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        //        {
        //            await sqlConnection.OpenAsync();
        //            string passwordhash = SHA256HashGenerator.GenerateHash(addUserRequest?.password);
        //            SqlCommand sqlCommand = new SqlCommand("UserRegister", sqlConnection);
        //            sqlCommand.CommandType = CommandType.StoredProcedure;
        //            sqlCommand.Parameters.Add(new SqlParameter("@UserName", addUserRequest?.UserName));
        //            sqlCommand.Parameters.Add(new SqlParameter("@Email", addUserRequest?.Email));
        //            sqlCommand.Parameters.Add(new SqlParameter("@Role", addUserRequest?.Role));
        //            sqlCommand.Parameters.Add(new SqlParameter("@PasswordHash", passwordhash));
        //            await sqlCommand.ExecuteNonQueryAsync();
        //        }
        //        return StatusCode(201, new { message = "Successfully added user." });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message, "addUser Error Message");
        //        _logger.LogError(ex.StackTrace, "addUser Error StackTrace");
        //        return StatusCode(500, new { message = "addUser failed" });
        //    }
        //}

       

    }
}
