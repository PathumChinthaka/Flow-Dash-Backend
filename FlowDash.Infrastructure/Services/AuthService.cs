using FlowDash.Application.Authentication.Commands.Login;
using FlowDash.Application.Authentication.Commands.RefreshToken;
using FlowDash.Application.Authentication.Commands.Register;
using FlowDash.Application.Authentication.Common;
using FlowDash.Application.Common.Interfaces.Service;

namespace FlowDash.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        public Task<AuthResult> LoginAsync(LoginCommand request)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResult> RefreshTokenAsync(RefreshTokenCommand request)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResult> RegisterAsync(RegisterCommand request)
        {
            throw new NotImplementedException();
        }
    }
}
