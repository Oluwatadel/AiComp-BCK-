using AiComp.Application.DTOs.RequestModel;
using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using System.Security.Claims;

namespace AiComp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> AddUserAsync(UserRequestModel user)
        {
            var newUser = new User(user.Email);
            newUser.AddPassword(user.Password);
            var userToBeReturned = await _userRepository.AddUserAsync(newUser);
            var changes = await _unitOfWork.SaveChanges();
            return changes > 0 ? userToBeReturned : null;
        }

        public async Task<User> UpdateUserEmailAsync(Guid userId, string email)
        {
            var user = await _userRepository.GetUser(a => a.Id == userId);
            user.UpdateEmail(email);
            var changes = await _unitOfWork.SaveChanges(); 
            return changes > 0 ? user : null;
        }

        public async Task<bool> UserExist(string email)
        {
            var userExist = await _userRepository.UserExist(email);
            return userExist;
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _userRepository.GetUser(x => x.Email == email);
            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUser(x => x.Id == userId);
            return user;
        }

        public async Task<ICollection<User>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return users;
        }
    }
}
