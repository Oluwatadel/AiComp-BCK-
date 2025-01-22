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
            var response = new BaseResponse<Profile>();

            try
            {
                // Check if phone number exists
                if (await CheckPhoneNumberAsync(user, model.PhoneNumber!))
                {
                    response.SetValues("Phone Number exists", false, null);
                    return response;
                }

                // Upload profile picture if provided
                string? profilePicUrl = "";
                if (model.ProfilePicture != null)
                {
                    var uploadResult = await _profilePicUpload.ProfilePicUpload(model.ProfilePicture);
                    if (!uploadResult.Status)
                    {
                        response.SetValues("Profile picture upload failed", false, null);
                        return response;
                    }
                    profilePicUrl = uploadResult.Data;
                }

                var unspecifiedDateTime = DateTime.Parse(model.Age.ToString());
                var utcDateTime = DateTime.SpecifyKind(unspecifiedDateTime, DateTimeKind.Utc);

                // Create and populate the profile
                var newProfile = new Profile(
                    model.FirstName,
                    model.LastName,
                    model.Gender,
                    model.Occupation,
                    model.Address,
                    model.PhoneNumber,
                    model.ContactOfNextOfKin,
                    model.FullNameOfNextOfKin,
                    profilePicUrl
                );

                newProfile.UpdateAge(utcDateTime);
                newProfile.SetUserObject(user);
                user.Profile = newProfile;

                // Persist the profile
                await _profileRepository.AddProfileAsync(newProfile);
                var changesSaved = await _unitOfWork.SaveChanges();

                if (changesSaved > 0)
                {
                    response.SetValues("Profile created successfully", true, newProfile);
                    return response;
                }

                response.SetValues("Profile was not created! Data was not persisted to the database", false, null);
                return response;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error creating profile: {ex.Message}");
                response.SetValues("An error occurred while creating the profile", false, null);
                return response;
            }
        }   

        public async Task<BaseResponse<Profile>> UpdateNewProfile(User user, ProfileUpdateRequestModel model)
        {
            try
            {
                var profile = await _profileRepository.GetProfileAsync(user.Id);
                var response = new BaseResponse<Profile>();
                var uploadResult = await _profilePicUpload.ProfilePicUpload(model.ProfilePicture);
                if (!uploadResult.Status)
                {
                    response.SetValues("Profile picture upload failed", false, null);
                    return response;
                }
                if(profile != null)
                {
                    profile.UpdateProfilePicture(uploadResult.Data);
                    profile.UpdateProfile(model);
                    var returnedProfile = _profileRepository.UpdateProfileAsync(profile);
                    var changes = await _unitOfWork.SaveChanges();
                    var baseResponse = new BaseResponse<Profile>();
                    if (changes > 0)
                    {
                        baseResponse.SetValues("Profile updated successfully", true, profile);
                        return baseResponse;
                    }

                    baseResponse.SetValues("Something went wrong, Profile was not updated!!! Data was not persisted to the dataBase", false, null);
                    return baseResponse;
                }
                else
                {
                    var basResponse = new BaseResponse<Profile>();
                    basResponse.SetValues("Profile not found", false, null);
                    return basResponse;
                }
                
            }
            catch(Exception ex)
            {
                var newBaseResponse = new BaseResponse<Profile>();
                newBaseResponse.SetValues(ex.Message, false, null);
                return newBaseResponse;
            }

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
                response.SetValues("Profile Pics is empty", false, "");
                return response;
            }
            var profilePic = await _profilePicUpload.ProfilePicUpload(profilePicture);
            if (!profilePic.Status)
            {
                response = profilePic;
                return response;
            }
            user.Profile!.UpdateProfilePicture(profilePic.Data!);
            var changes = await _unitOfWork.SaveChanges();
            if (changes == 0)
            {
                response.SetValues("Upload fail!!! Internal Error", false, "");
                return response;
            }
            response.SetValues("Upload successful", true, "");
            return response;
        }

        public async Task<BaseResponse<Profile>> GetProfile(Guid userId)
        {
            var userProfile = await _profileRepository.GetProfileAsync(userId);
            var response = new BaseResponse<Profile>();
            if (userProfile == null)
            {
                response.SetValues("Profile not found", false, null);
                return response;
            }
            response.SetValues("Profile found", true, userProfile);
            return response;
        }
        
        public async Task<BaseResponse<string>> GetProfilePic(User user)
        {
            var userProfile = await _profileRepository.GetProfileAsync(user.Id);
            var response = new BaseResponse<string>();
            if (userProfile == null)
            {
                response.SetValues("Profile pic not found", false, null);
                return response;
            }
            response.SetValues("Profile pic found", true, userProfile.ProfilePicture!);
            return response;
        }

        private async Task<bool> CheckPhoneNumberAsync(User user, string phoneNumber)
        {
            if (user.Profile == null)
            {
                return false;
            }
            return await Task.FromResult(user.Profile.PhoneNumber == phoneNumber ? true : false);
        }

        //private  int CalculateAge(DateTime dateOfBirth)
        //{
        //    DateTime today = DateTime.Now;
        //    int age = today.Year - dateOfBirth.Year;

        //    // Check if the birthday has already occurred this year
        //    if (today < dateOfBirth.AddYears(age))
        //    {
        //        age--; // If the birthday hasn't occurred yet, subtract one year
        //    }
        //    else
        //    {
        //        return 0;
        //    }    

        //    return age;
        //}
    }
}
