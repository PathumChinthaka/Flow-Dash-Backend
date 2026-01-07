namespace FlowDash.Contract.Authentication.Response
{
    public record AuthResponse
    (
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresOn
    );
}
