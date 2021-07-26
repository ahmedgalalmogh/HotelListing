using HotelListing.Data;
using HotelListing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApiUser> _usermanager;
        private readonly IConfiguration _configuration;
        private  ApiUser _user;
        public AuthManager(UserManager<ApiUser> usermanager, IConfiguration configuration)
        {
            _usermanager = usermanager;
            _configuration = configuration;
        }

        public async Task<string> CreateToken()
        {
            var siginigCredantials = GetSigningCredantials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(siginigCredantials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials siginigCredantials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("jwt");
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));
            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
               expires:expiration,
                signingCredentials: siginigCredantials
                ) ;
            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, _user.UserName) };
            var roles = await _usermanager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        private SigningCredentials GetSigningCredantials()
        {
            var key = _configuration.GetSection("jwt").GetSection("key").Value;
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<bool> ValidateUser(LoginUserDTO userDto)
        {
            _user = await _usermanager.FindByEmailAsync(userDto.Email);
            return (_user != null && await _usermanager.CheckPasswordAsync(_user, userDto.Password));
        }
    }
}
