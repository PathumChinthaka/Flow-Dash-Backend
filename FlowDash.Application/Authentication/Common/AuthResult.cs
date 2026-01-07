using Microsoft.AspNetCore.Http;

namespace FlowDash.Application.Authentication.Common
{
    public record AuthResult
    (
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresOn,
        string Email,
        string FirstName,
        string LastName,
        CookieOptions CookieTokenExpiaryOptions
    );
}
