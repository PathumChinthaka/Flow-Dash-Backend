using FlowDash.Application.Common.Interfaces;
using FlowDash.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowDash.Infrastructure.Repositories.Auth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly FlowDashDbContext _dbContext;

        public RefreshTokenRepository(FlowDashDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshToken> Create(RefreshToken request)
        {
            _dbContext.RefreshTokens.Add(request);
            await _dbContext.SaveChangesAsync();

            return request;
        }

        public async Task<RefreshToken?> GetByToken(string token)
        {
            return await _dbContext.RefreshTokens
                .Where(x => x.Token == token && !x.IsRevoked)
                .Include(x => x.User)
                .FirstOrDefaultAsync();
        }

        public async Task<RefreshToken> Delete(RefreshToken request)
        {
            request.IsRevoked = true;

            _dbContext.RefreshTokens.Update(request);
            await _dbContext.SaveChangesAsync();

            return request;
        }

        public async Task<List<RefreshToken>> GetListByUserId(int userId)
        {
            return await _dbContext.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .Include(x => x.User)
                .ToListAsync();
        }
    }
}
