using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using MVCTaskManager.Common;
using MVCTaskManager.Models;
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
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProjectsController> _logger;
        private readonly string connectionString;

        public AccountController(IConfiguration configuration, ILogger<ProjectsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            connectionString = _configuration.GetConnectionString("DefaultConnection"); ;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                DataSet userDetails = new DataSet();
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string passwordhash = SHA256HashGenerator.GenerateHash(loginRequest?.Password);
                    SqlCommand sqlCommand = new SqlCommand("GetUserDetailsByEmailAndPassword", sqlConnection);
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Email", loginRequest?.Email));
                    sqlCommand.Parameters.Add(new SqlParameter("@Password", passwordhash));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(userDetails);
                    if (userDetails != null && userDetails.Tables.Count > 0 && userDetails.Tables[0].Rows.Count > 0)
                    {
                        // Convert DataTable to List<LoginResponse>
                        List<LoginResponse> loginResponse = await CommonHelper.ConvertDataTableToList<LoginResponse>(userDetails.Tables[0]);
                        var jwtToken = GenerateJwtToken(loginResponse[0].UserId, loginResponse[0].Role);
                        loginResponse[0].token = jwtToken;
                        return StatusCode(200, new { message = "login Successful", data = loginResponse });
                    }
                    else
                    {
                        return StatusCode(400, new { message = "no user found", data = new List<LoginResponse>() });
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Login Error Message");
                _logger.LogError(ex.StackTrace, "Login Error StackTrace");
                return StatusCode(500, new { message = "login failed", data = new List<LoginResponse>() });
            }
        }

        [Authorize]
        [HttpPost("addUser")]
        public async Task<IActionResult> addUser([FromBody] AddUserRequest addUserRequest) 
        {
            try
            {
                if (addUserRequest != null && string.IsNullOrEmpty(addUserRequest.Email) && string.IsNullOrEmpty(addUserRequest.UserName) && string.IsNullOrEmpty(addUserRequest.Role) && string.IsNullOrEmpty(addUserRequest.password))
                {
                    return StatusCode(500, new { message="request parameter can't be null or empty"});
                }
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    string passwordhash = SHA256HashGenerator.GenerateHash(addUserRequest?.password);
                    SqlCommand sqlCommand = new SqlCommand("UserRegister", sqlConnection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@UserName", addUserRequest?.UserName));
                    sqlCommand.Parameters.Add(new SqlParameter("@Email", addUserRequest?.Email));
                    sqlCommand.Parameters.Add(new SqlParameter("@Role", addUserRequest?.Role));
                    sqlCommand.Parameters.Add(new SqlParameter("@PasswordHash", passwordhash));
                    await sqlCommand.ExecuteNonQueryAsync();
                }
                return StatusCode(201, new { message = "Successfully added user." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "addUser Error Message");
                _logger.LogError(ex.StackTrace, "addUser Error StackTrace");
                return StatusCode(500, new { message = "addUser failed" });
            }
        }

        private string GenerateJwtToken(string UserId,string role)
        {
            // Create claims based on user info
            var claims = new[]
            {
              new Claim(JwtRegisteredClaimNames.UniqueName, UserId),
              new Claim("role", role),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            

            // Get the secret key from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Generate the token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            // Return the token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
