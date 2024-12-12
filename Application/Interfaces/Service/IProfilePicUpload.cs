using AiComp.Application.DTOs;

namespace AiComp.Application.Interfaces.Service
{
    public interface IProfilePicUpload
    {
        public Task<BaseResponse<string>> ProfilePicUpload(IFormFile pics);
    }
}
