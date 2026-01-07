using FlowDash.Domain.Entities;

namespace FlowDash.Application.Common.Interfaces.Service
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        RefreshToken CreateRefreshToken();
    }
}
