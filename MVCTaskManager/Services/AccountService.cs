using MVCTaskManager.Common;
using MVCTaskManager.JWT;
using MVCTaskManager.Models;
using MVCTaskManager.Repositories.Interfaces;
using MVCTaskManager.Services.Interfaces;
using System.Collections.Generic;
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
    }
}
