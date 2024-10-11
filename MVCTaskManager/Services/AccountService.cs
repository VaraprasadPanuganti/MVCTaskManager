using MVCTaskManager.Common;
using MVCTaskManager.JWT;
using MVCTaskManager.Models;
using MVCTaskManager.Repositories.Interfaces;
using MVCTaskManager.Services.Interfaces;
using MVCTaskManager.SHA256Gen;
using System.Data;

namespace MVCTaskManager.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        private readonly IConfiguration configuration;
        public AccountService(IAccountRepository _accountRepository, IConfiguration _configuration)
        {
            accountRepository = _accountRepository;
            configuration = _configuration;
        }

        public async Task<LoginResponse> FindUserbyEmail(PasswordResetRequest passwordResetRequest)
        {
            DataTable dtUser = await accountRepository.FindUserbyEmail(passwordResetRequest);
            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                LoginResponse loginResponse = await CommonHelper.ConvertDataTableClass<LoginResponse>(dtUser);
      
                return loginResponse;
            }
            else
            {
                return new LoginResponse();
            }

        }

        public async Task<LoginResponse> GetUserById(string UserId)
        {
            DataTable dtUser = await accountRepository.GetUserById(UserId);
            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                LoginResponse loginResponse = await CommonHelper.ConvertDataTableClass<LoginResponse>(dtUser);

                return loginResponse;
            }
            else
            {
                return new LoginResponse();
            }

        }

        public async Task<List<LoginResponse>> Login(LoginRequest loginRequest)
        {
            DataTable userDetails = await accountRepository.Login(loginRequest);
            if (userDetails != null && userDetails.Rows.Count > 0)
            {
                // Convert DataTable to List<LoginResponse>
                List<LoginResponse> loginResponse = await CommonHelper.ConvertDataTableToList<LoginResponse>(userDetails);
                loginResponse[0].token = new JwtTokenGenerator(configuration).GenerateJwtTokenForSerice(loginResponse[0].UserId, loginResponse[0].Role);
                return loginResponse;
            }
            else
            {
                return new List<LoginResponse>();
            }
        }

        public async Task<int> UpdateUserPassword(string userId, string password)
        {
            string passwordhash = SHA256HashGenerator.GenerateHash(password);
            int result = await accountRepository.UpdateUserPassword(userId, passwordhash);
            return result;
        }
    }
}
