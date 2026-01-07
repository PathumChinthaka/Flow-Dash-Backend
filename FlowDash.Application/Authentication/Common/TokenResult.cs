namespace FlowDash.Application.Authentication.Common
{
    public record class TokenResult(
        string Value,
        DateTime ExpiresOn
    );
}
