using FlowDash.Domain.Entities;

namespace FlowDash.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task<User> Update(User user);
        Task<User> DeactivateUser(User user);
        Task<User?> Get(int id);
        Task<User?> GetByEmail(string email);
    }
}
