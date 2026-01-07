namespace FlowDash.Contract.Authentication.Request
{
    public record RegisterRequest(string FirstName, string LastName, string Email, string Password);
}
