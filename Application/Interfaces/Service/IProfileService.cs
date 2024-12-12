using AiComp.Application.DTOs;
using AiComp.Application.DTOs.RequestModel;
using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface IProfileService
    {
        public Task<BaseResponse<Profile>> CreateNewProfile(User user, ProfileCreateModel model);
        public Task<BaseResponse<Profile>> UpdateNewProfile(User user, ProfileUpdateRequestModel model);
        public Task<bool> CheckProfileExist(User user);
        public Task<BaseResponse<string>> ChangeProfilePics(User user, IFormFile profilePicture);
        public Task<BaseResponse<Profile>> GetProfile(Guid userId);
        public Task<BaseResponse<string>> GetProfilePic(User user);
    }
}
