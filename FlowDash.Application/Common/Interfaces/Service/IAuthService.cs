using FlowDash.Application.Authentication.Commands.Login;
using FlowDash.Application.Authentication.Commands.RefreshToken;
using FlowDash.Application.Authentication.Commands.Register;
using FlowDash.Application.Authentication.Common;

namespace FlowDash.Application.Common.Interfaces.Service
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterCommand request);
        Task<AuthResult> LoginAsync(LoginCommand request);
        Task<AuthResult> RefreshTokenAsync(RefreshTokenCommand request);
    }
}
