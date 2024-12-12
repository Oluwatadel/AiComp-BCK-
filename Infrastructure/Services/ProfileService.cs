using AiComp.Application.DTOs;
using AiComp.Application.DTOs.RequestModel;
using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IProfilePicUpload _profilePicUpload;
        private readonly IUnitOfWork _unitOfWork;
        public ProfileService(IUserRepository userRepository, IProfileRepository profileRepository, IProfilePicUpload profilePicUpload, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _profilePicUpload = profilePicUpload;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<Profile>> CreateNewProfile(User user, ProfileCreateModel model)
        {
            var phoneNumberExist = await CheckPhoneNumberAsync(user, model.PhoneNumber!);
            if (phoneNumberExist)
            {
                var newBaseResponse = new BaseResponse<Profile>();
                newBaseResponse.SetValues("UnSuccessfull", "Phone Number exist", null);
                return newBaseResponse;
            }

            //profile pics
            var upload = new BaseResponse<string>();

            if (model.ProfilePicture != null)
            {
               var newUpload = await _profilePicUpload.ProfilePicUpload(model.ProfilePicture);
                if (newUpload.Data == string.Empty || newUpload.Status != "Success")
                {
                    var newBaseResponse = new BaseResponse<Profile>();
                    newBaseResponse.SetValues(newUpload.Message, newUpload.Status, null);
                    return newBaseResponse;
                }
                upload = newUpload;
            }
            int age = CalculateAge(model.Age);
            var profile = new Profile(model.FirstName, model.LastName, age, model.Gender, model.Occupation, model.Address, model.PhoneNumber, model.ContactOfNextOfKin, model.FullNameOfNextOfKin, upload.Data);
            profile.SetUserObject(user);
            await _profileRepository.AddProfileAsync(profile);
            
            var changes = await _unitOfWork.SaveChanges();
            var baseResponse = new BaseResponse<Profile>();

            if (changes > 0)
            {
                baseResponse.SetValues("Successfull", "Profile Created successfully", profile);
                return baseResponse;
            }

            baseResponse.SetValues("UnSuccessfull", "Profile was not created!!! Data was not persisted to the dataBase", null);
            return baseResponse;

        }   

        public async Task<BaseResponse<Profile>> UpdateNewProfile(User user, ProfileUpdateRequestModel model)
        {
            var profile = await _profileRepository.GetProfileAsync(user.Id);
            profile.UpdateProfile(model);

            var changes = await _unitOfWork.SaveChanges();
            var baseResponse = new BaseResponse<Profile>();
            if (changes > 0)
            {
                baseResponse.SetValues("Successfull", "Profile updated successfully", profile);
                return baseResponse;
            }

            baseResponse.SetValues("UnSuccessfull", "Something went wrong, Profile was not updated!!! Data was not persisted to the dataBase", null);
            return baseResponse;

        }

        private async Task<bool> CheckPhoneNumberAsync(User user, string phoneNumber)
        {
            if (user.Profile == null)
            {
                return false;
            }
            return await Task.FromResult(user.Profile.PhoneNumber == phoneNumber ? true : false);
        }

        public async Task<bool> CheckProfileExist(User user)
        {
            return await Task.FromResult(user.Profile == null ? false : true);
        }

        public async Task<BaseResponse<string>> ChangeProfilePics(User user, IFormFile profilePicture)
        {
            var response = new BaseResponse<string>();

            if (profilePicture == null)
            {
                response.SetValues("Profile Pics is empty", "Unsuccessfull", "");
                return response;
            }
            var profilePic = await _profilePicUpload.ProfilePicUpload(profilePicture);
            if (profilePic.Data == string.Empty)
            {
                response.SetValues("Upload fail!!! Internal Error", "Unsuccessfull", "");
                return response;
            }
            user.Profile!.UpdateProfilePicture(profilePic.Data!);
            var changes = await _unitOfWork.SaveChanges();
            if (changes == 0)
            {
                response.SetValues("Upload fail!!! Internal Error", "Unsuccessfull", "");
                return response;
            }
            response = profilePic;
            return response;
        }

        public async Task<BaseResponse<Profile>> GetProfile(Guid userId)
        {
            var userProfile = await _profileRepository.GetProfileAsync(userId);
            var response = new BaseResponse<Profile>();
            if (userProfile == null)
            {
                response.SetValues("Profile not found", "Unsuccessful", null);
                return response;
            }
            response.SetValues("Profile found", "Successful", userProfile);
            return response;
        }
        
        public async Task<BaseResponse<string>> GetProfilePic(User user)
        {
            var userProfile = await _profileRepository.GetProfileAsync(user.Id);
            var response = new BaseResponse<string>();
            if (userProfile == null)
            {
                response.SetValues("Profile pic not found", "Unsuccessful", null);
                return response;
            }
            response.SetValues("Profile pic found", "Successful", userProfile.ProfilePicture!);
            return response;
        }

        private  int CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Now;
            int age = today.Year - dateOfBirth.Year;

            // Check if the birthday has already occurred this year
            if (today < dateOfBirth.AddYears(age))
            {
                age--; // If the birthday hasn't occurred yet, subtract one year
            }

            return age;
        }
    }
}
