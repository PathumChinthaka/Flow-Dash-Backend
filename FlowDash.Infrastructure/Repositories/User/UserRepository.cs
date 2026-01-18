using FlowDash.Application.Common.Interfaces;
using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Application.Common.Pagination;
using FlowDash.Application.User.Queries.GetList;
using Mapster;
using Microsoft.EntityFrameworkCore;
using UserModel = FlowDash.Domain.Entities.User;

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

        public async Task<UserModel> Create(UserModel user, CancellationToken cancellationToken)
        {
            user.IsActive = true;
            user.CreatedOn = _dateTimeProvider.UtcNow;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<UserModel> Update(UserModel user, CancellationToken cancellationToken)
        {
            user.UpdatedOn = _dateTimeProvider.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<UserModel> DeactivateUser(UserModel user, CancellationToken cancellationToken)
        {
            user.IsActive = false;
            user.UpdatedOn = _dateTimeProvider.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<UserModel?> Get(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .ProjectToType<UserModel>()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<UserModel?> GetByEmail(string email, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PaginatedResult<UserModel>> GetList(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var usersQuery = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                usersQuery = usersQuery.Where(u =>
                    u.FirstName.Contains(query.Search) ||
                    u.LastName.Contains(query.Search));
            }

            if (query.IsActive.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.IsActive == query.IsActive);
            }

            var totalCount = await usersQuery.CountAsync();

            var users = await usersQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<UserModel>
            {
                Items = users,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
