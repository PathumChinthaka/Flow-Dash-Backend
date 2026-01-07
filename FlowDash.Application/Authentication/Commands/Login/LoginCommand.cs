using FlowDash.Application.Authentication.Common;
using MediatR;

namespace FlowDash.Application.Authentication.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
}
