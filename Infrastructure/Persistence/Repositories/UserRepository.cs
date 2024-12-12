using AiComp.Application.Interfaces.Repository;
using AiComp.Domain.Entities;
using AiComp.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AiCompDBContext _dbContext;

        public UserRepository(AiCompDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            return user;
        }

        public async Task<ICollection<User>> GetAllUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetUser(Expression<Func<User, bool>> pred)
        {
            var user = await _dbContext.Users.Include(a => a.Profile).SingleOrDefaultAsync(pred);
            return user;
        }
        public async Task<User> UpdateUser(User user)
        {
            _dbContext.Users.Update(user);
            await Task.CompletedTask;
            return user;   
        }

        public async Task<bool> UserExist(string email)
        {
            var isExist = _dbContext.Users.Any(u => u.Email == email);
            return isExist;
        }
    }
}
