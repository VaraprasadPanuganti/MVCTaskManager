using Microsoft.AspNetCore.Mvc;
using MVCTaskManager.JWT;
using MVCTaskManager.Models;
using MVCTaskManager.Services;
using MVCTaskManager.Services.Interfaces;

namespace MVCTaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService accountService;
        private readonly IConfiguration configuration;

        public AccountController(ILogger<AccountController> logger, IAccountService _accountService, IConfiguration _configuration)
        {
            _logger = logger;
            accountService = _accountService;
            configuration = _configuration;
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

        [HttpPost("passwordResetRequest")]
        public async Task<IActionResult> PasswordResetRequest([FromBody] PasswordResetRequest passwordResetRequest)
        {
            try
            {
                LoginResponse loginResponse = await accountService.FindUserbyEmail(passwordResetRequest);

                if (string.IsNullOrEmpty(loginResponse.UserId) && string.IsNullOrEmpty(loginResponse.SecurityStamp) && string.IsNullOrEmpty(loginResponse.UserName))
                {
                    // You may return OK to avoid disclosing that the email is not registered.
                    return StatusCode(200, new { message = "If the email is valid, a reset link has been sent." });
                }
                else
                {
                    var resetToken = await new JwtTokenGenerator(configuration).GeneratePasswordResetToken(loginResponse.UserId, loginResponse.SecurityStamp);
                    // Construct the reset URL
                    var resetUrl = $"{passwordResetRequest.Url}?token={resetToken}";
                    await new MailService(configuration).SendMail(loginResponse.UserName, resetUrl, loginResponse.Email);
                    return StatusCode(200, new { message = "Shortly you will receive a mail to update the password" });
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "PasswordResetRequest Error Message");
                _logger.LogError(ex.StackTrace, "PasswordResetRequest Error StacakTrace");
                return StatusCode(500, new { message = "PasswordResetRequest failed" });
            }

        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            try
            {
                var response = await new JwtTokenGenerator(configuration).ValidatePasswordResetToken(resetPassword.resetToken);
                if (response.isvalidToken)
                {
                    int isPassWordUpdated = await accountService.UpdateUserPassword(response.UserId, resetPassword.resetPassword);
                    if (isPassWordUpdated == 1)
                    {
                        return StatusCode(200, new { message = "Password Updated Successfully." });
                    }
                    else 
                    {
                        return StatusCode(200, new { message = "Password Updation failed." });
                    }
                    
                }
                else
                {
                    return StatusCode(500, new { message = response.Message });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "ResetPassword Error Message");
                _logger.LogError(ex.StackTrace, "ResetPassword Error StacakTrace");
                return StatusCode(500, new { message = "ResetPassword failed" });
            }
        }

        [HttpPost("validatePasswordResetToken")]
        public async Task<IActionResult> ValidateToken(ValidatePasswordResetTokenRequest validatePasswordResetTokenRequest)
        {
            try
            {
                var response = await new JwtTokenGenerator(configuration).ValidatePasswordResetToken(validatePasswordResetTokenRequest.Token);
                if (response.isvalidToken)
                {
                    return StatusCode(200, new { message = response.Message,isValidToken= response.isvalidToken });
                }
                else
                {
                    return StatusCode(200, new { message = response.Message, isValidToken = response.isvalidToken });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "ValidateToken Error Message");
                _logger.LogError(ex.StackTrace, "ValidateToken Error StacakTrace");
                return StatusCode(500, new { message = "ValidateToken failed" });
            }
        }


    }
}
