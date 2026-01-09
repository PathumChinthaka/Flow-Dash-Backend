using FlowDash.Application.Authentication.Common;
using FlowDash.Application.Common.Interfaces;
using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FlowDash.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public TokenService(IConfiguration config, IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _config = config;
        }

        public TokenResult CreateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!)
            );

            var tokenExpireOn = DateTime.UtcNow.AddMinutes(
                int.Parse(_config["JwtSettings:AccessTokenMinutes"]!)
            );

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: tokenExpireOn,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return new TokenResult(token.ToString(), tokenExpireOn);
        }

        public async Task<RefreshToken> CreateRefreshToken(int userId)
        {
            string tokenWithSpecialCharacters = $"{Guid.NewGuid()}-{Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))}";
            string tokenWithoutSpecialCharacters = Regex.Replace(tokenWithSpecialCharacters, "[^0-9a-zA-Z]+", "");

            var refreshToken =  new RefreshToken
            {
                Token = tokenWithoutSpecialCharacters,
                UserId = userId,
                IsRevoked = false,
                ExpiresOn = DateTime.UtcNow.AddDays(
                    int.Parse(_config["JwtSettings:RefreshTokenDays"]!)
                )
            };

            await _refreshTokenRepository.Create(refreshToken);
            return refreshToken;
        }

        public CookieOptions SetRefreshTokenExpiary(RefreshToken refreshToken)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = refreshToken.ExpiresOn
            };

            return cookieOption;
        }
    }
}
