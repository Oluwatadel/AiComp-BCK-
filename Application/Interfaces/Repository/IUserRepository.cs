using AiComp.Domain.Entities;
using System.Linq.Expressions;

namespace AiComp.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        public Task<User> AddUserAsync(User user);
        public Task<ICollection<User>> GetAllUsers();
        public Task<User> GetUser(Expression<Func<User, bool>> pred);
        public Task<User> UpdateUser(User user);
        public Task<bool> UserExist(string email);
    }
}
