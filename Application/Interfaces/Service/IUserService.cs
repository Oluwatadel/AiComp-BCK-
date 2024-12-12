using AiComp.Application.DTOs.RequestModel;
using AiComp.Application.Interfaces.Repository;
using AiComp.Domain.Entities;
using System.Threading.Tasks;

namespace AiComp.Application.Interfaces.Service
{
    public interface IUserService
    {
        public Task<User> AddUserAsync(UserRequestModel user);
        public Task<User> UpdateUserEmailAsync(Guid userId, string email);
        public Task<bool> UserExist(string email);
        public Task<User> GetUserAsync(string email);
        public Task<User> GetUserByIdAsync(Guid userId);
        public Task<ICollection<User>> GetAllUsers();
    }
}
