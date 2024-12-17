using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AiComp.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityService(IConfiguration configuration, IUserRepository userRepository, IProfileRepository profileRepository, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            var user = await IsValidEmail(email);
            if(user == null) return false;
            var passwordIsCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (passwordIsCorrect) return true;
            return false;
        }

        public async Task<string> GenerateToken(User user)
        {
            var security = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(security, SecurityAlgorithms.HmacSha256);
            var profile = await _profileRepository.GetProfileAsync(user.Id);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            
            if(profile != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, $"{profile.LastName} {profile.FirstName}"));
            }

            //Generate Token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credential
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<User> IsValidEmail(string email)
        {
            var userExist = await _userRepository.GetUser(a => a.Email == email);
            return userExist != null ? userExist : null;
        }

        public async Task<User> GetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext?.User == null)
            {
                return null;
            }

            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var currentUserId))
            {
                return null;
            }

            return await _userRepository.GetUser(a => a.Id == currentUserId);
        }
    }
}
