using FlowDash.Application.Common.Interfaces;
using UserModel = FlowDash.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using FlowDash.Application.Common.Interfaces.Service;

namespace FlowDash.Infrastructure.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly FlowDashDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserRepository(FlowDashDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<UserModel> Create(UserModel user)
        {
            user.IsActive = true;
            user.CreatedOn = _dateTimeProvider.UtcNow;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<UserModel> Update(UserModel user)
        {
            user.UpdatedOn = _dateTimeProvider.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<UserModel> DeactivateUser(UserModel user)
        {
            user.IsActive = false;
            user.UpdatedOn = _dateTimeProvider.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<UserModel?> Get(int id)
        {
            return await _dbContext.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<UserModel?> GetByEmail(string email)
        {
            return await _dbContext.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();
        }
    }
}
