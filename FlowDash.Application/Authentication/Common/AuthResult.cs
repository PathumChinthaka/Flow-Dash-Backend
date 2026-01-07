namespace FlowDash.Application.Authentication.Common
{
    public record AuthResult
    (
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresOn
    );
}
