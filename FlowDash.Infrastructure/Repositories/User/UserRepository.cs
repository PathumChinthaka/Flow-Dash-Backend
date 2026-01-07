using FlowDash.Application.Common.Interfaces;
using UserModel = FlowDash.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace FlowDash.Infrastructure.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly FlowDashDbContext _dbContext;

        public UserRepository(FlowDashDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserModel> Create(UserModel user)
        {
            if (user.Id != 0) return user;  

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<UserModel> Update(UserModel user)
        {
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
