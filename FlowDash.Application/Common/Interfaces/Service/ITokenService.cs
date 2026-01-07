using FlowDash.Application.Authentication.Common;
using FlowDash.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FlowDash.Application.Common.Interfaces.Service
{
    public interface ITokenService
    {
        TokenResult CreateAccessToken(User user);
        RefreshToken CreateRefreshToken();
        CookieOptions SetRefreshTokenExpiary(RefreshToken refreshToken);
    }
}
