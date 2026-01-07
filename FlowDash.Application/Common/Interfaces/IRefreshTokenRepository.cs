using FlowDash.Domain.Entities;

namespace FlowDash.Application.Common.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> Create(RefreshToken request);
        Task<RefreshToken?> GetByToken(string token);
        Task<RefreshToken> Delete(RefreshToken request);
        Task<List<RefreshToken>> GetListByUserId(int userId);
    }
}
