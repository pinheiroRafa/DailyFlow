using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthApi.Domain;
using AuthApi.Dtos;
using AuthApi.Interfaces;
using AuthApi.Utils;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services 
{
    public class AuthService : IAuthService
    {

        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public AuthService(IUserRepository userRepository, ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
        }

        public async Task<LoginResponse> Login(string username, string password) 
        {
            var user = await this._userRepository.GetUserByEmail(username) ?? throw new ApiError("user not found", 401);
            var company = await this._companyRepository.GetByOwnerId(user.Id) ?? throw new ApiError("company not found", 401);
            var claims = new List<Claim>
            {
                new("userId", user.Id),
                new("companyId", company.Id)
            };
            var rsa = KeyManager.LoadPrivateKey(Path.Combine(AppContext.BaseDirectory, "private.key"));
            var token = new JwtSecurityToken(
                issuer: "SeuIssuer",
                audience: "SeuAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return new LoginResponse { AccessToken = accessToken };
        }
    }
}