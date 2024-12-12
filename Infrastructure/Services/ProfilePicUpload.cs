using AiComp.Application.DTOs;
using AiComp.Application.Interfaces.Service;

namespace AiComp.Infrastructure.Services
{
    public class ProfilePicUpload : IProfilePicUpload
    {

        async Task<BaseResponse<string>> IProfilePicUpload.ProfilePicUpload(IFormFile pics)
        {
            if (pics == null)
            {
                return null;
            }

            List<string> validExtension = new List<string>() {".jpg", ".png", ".jpeg"};
            string fileExtension = Path.GetExtension(pics.FileName);
            if(!validExtension.Contains(fileExtension))
            {
                var response = new BaseResponse<string>();
                response.SetValues($"Extension is not valid ({string.Join(',', validExtension)})", "Upload failed","");
                return response;
            }

            //check file size <= 5mb
            long size = pics.Length;
            if(size > (5 * 1024 * 1024))
            {
                var response = new BaseResponse<string>();
                response.SetValues($"File size is greater than 5mb", "Upload failed", "");
                return response;
            }

            //Name changing
            var newFileName = Guid.NewGuid().ToString() + fileExtension;
            
            //
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ProfilePics");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, newFileName);

            using (var picToUpload = new FileStream(filePath, FileMode.Create))
            {
                await pics.CopyToAsync(picToUpload);
            }

            var baseResponse = new BaseResponse<string>();
            baseResponse.SetValues($"File uploaded successfully", "Success", $"{filePath}");
            return baseResponse;
        }
    }
}
