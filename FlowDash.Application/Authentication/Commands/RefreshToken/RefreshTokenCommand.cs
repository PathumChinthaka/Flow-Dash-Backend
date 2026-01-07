using FlowDash.Application.Authentication.Common;
using MediatR;

namespace FlowDash.Application.Authentication.Commands.RefreshToken
{
    public record RefreshTokenCommand(string Token) : IRequest<AuthResult>;
}
