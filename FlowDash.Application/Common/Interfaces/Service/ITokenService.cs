using FlowDash.Application.Authentication.Common;
using UserModel = FlowDash.Domain.Entities.User;
using RefreshTokenModel = FlowDash.Domain.Entities.RefreshToken;
using Microsoft.AspNetCore.Http;

namespace FlowDash.Application.Common.Interfaces.Service
{
    public interface ITokenService
    {
        TokenResult CreateAccessToken(UserModel user);
        Task<RefreshTokenModel> CreateRefreshToken(int userId);
        CookieOptions SetRefreshTokenExpiary(RefreshTokenModel refreshToken);
    }
} 
