using Microsoft.IdentityModel.Tokens;
using MVCTaskManager.Models;
using MVCTaskManager.Repositories;
using MVCTaskManager.Repositories.Interfaces;
using MVCTaskManager.Services;
using MVCTaskManager.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVCTaskManager.JWT
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtTokenForSerice(string UserId, string role)
        {
            // Create claims based on user info
            var claims = new[]
            {
              new Claim("userId", UserId.ToString()),
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

        public async Task<string> GeneratePasswordResetToken(string UserId, string SecurityStamp)
        {
            // Create claims based on user info
            var claims = new[]
            {
                  new Claim("userId", UserId),
                  new Claim("SecurityStamp", SecurityStamp),
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
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds
            );

            // Return the token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ValidatePasswordResetTokenResponse> ValidatePasswordResetToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                // Validate token and get claims principal
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                //Get the user ID and security stamp from the claims
                var userIdClaim = principal.FindFirst("userId")?.Value;
                var securityStampClaim = principal.FindFirst("SecurityStamp")?.Value;

                // Fetch the current user details to check the security stamp
                IAccountRepository accountRepository = new AccountRepository(_configuration);
                IAccountService accountService = new AccountService(accountRepository, _configuration);

                var user = await accountService.GetUserById(userIdClaim);
                if (user == null || user?.SecurityStamp != securityStampClaim)
                {
                    return new ValidatePasswordResetTokenResponse()
                    {
                        isvalidToken = true,
                        Message = "Invalid token: Security stamp mismatch.",
                    };
                }
                return new ValidatePasswordResetTokenResponse()
                {
                    isvalidToken = true,
                    Message = "Valid Token",
                    UserId = userIdClaim
                };

            }
            catch (SecurityTokenExpiredException)
            {
                return new ValidatePasswordResetTokenResponse()
                {
                    isvalidToken = false,
                    Message = "Token has expired."
                };

            }
            catch (SecurityTokenException)
            {
                return new ValidatePasswordResetTokenResponse()
                {
                    isvalidToken = false,
                    Message = "Invalid token."
                };
                throw new SecurityTokenException("Invalid token.");

            }
        }
    }
}
