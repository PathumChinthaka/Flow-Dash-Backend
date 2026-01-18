using FlowDash.Application.Common.Pagination;
using FlowDash.Application.User.Queries.GetList;
using UserModel = FlowDash.Domain.Entities.User;

namespace FlowDash.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<UserModel> Create(UserModel user, CancellationToken cancellationToken);
        Task<UserModel> Update(UserModel user, CancellationToken cancellationToken);
        Task<UserModel> DeactivateUser(UserModel user, CancellationToken cancellationToken);
        Task<UserModel?> Get(int id, CancellationToken cancellationToken);
        Task<UserModel?> GetByEmail(string email, CancellationToken cancellationToken);
        Task<PaginatedResult<UserModel>> GetList(GetUsersQuery query, CancellationToken cancellationToken);
    }
}
