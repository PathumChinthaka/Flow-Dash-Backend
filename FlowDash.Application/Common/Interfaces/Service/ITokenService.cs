using FlowDash.Application.Authentication.Common;
using FlowDash.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FlowDash.Application.Common.Interfaces.Service
{
    public interface ITokenService
    {
        TokenResult CreateAccessToken(User user);
        Task<RefreshToken> CreateRefreshToken(int userId);
        CookieOptions SetRefreshTokenExpiary(RefreshToken refreshToken);
    }
} 
