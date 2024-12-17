using AiComp.Application.DTOs.RequestModel;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AiComp.Controllers
{
    [Route("api/p")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;

        public ProfileController(IProfileService profileService, IUserService userService, IIdentityService identityService)
        { 
            _profileService = profileService;
            _userService = userService;
            _identityService = identityService;
        }


        [HttpPost("createprofile")]
        public async Task<IActionResult> AddProfile([FromForm] ProfileCreateModel createModel)
        {
            var currentUser = await _identityService.GetCurrentUser();
            var userProfile = await _profileService.CreateNewProfile(currentUser, createModel);
            if(userProfile.Data == null)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = userProfile.Message,
                    statusCode = 401
                });
            }
            return Created("", new
            {
                status = $"{userProfile.Status}",
                message = $"{userProfile.Message}",
                data = new
                {
                    userProfile.Data.LastName,
                    userProfile.Data.FirstName,
                    userProfile.Data.Address,
                    userProfile.Data.UserId,
                    userProfile.Data.Gender,
                    userProfile.Data.ProfilePicture,
                    userProfile.Data.DateOfBirth,
                    userProfile.Data.ContactOfNextOfKin,
                    userProfile.Data.FullNameOfNextOfKin,
                    userProfile.Data.PhoneNumber
                }
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileUpdateRequestModel model)
        {
            var user = await _identityService.GetCurrentUser();
            var profileResponse = await _profileService.UpdateNewProfile(user, model);
            if (profileResponse.Data == null)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = profileResponse.Message,
                    statusCode = 401
                });
            }
            return Created("", new
            {
                status = "success",
                message = "Registration Successfull",
                data = new
                {
                    profile = new
                    {
                        profileResponse.Data.FirstName,
                        profileResponse.Data.LastName,
                        profileResponse.Data.PhoneNumber,
                        profileResponse.Data.Address,
                        profileResponse.Data.Gender,
                        profileResponse.Data.Id
                    }
                }
            });
        }

        [HttpPut("profilepicture")]
        public async Task<IActionResult> ChangeProfilePciture(IFormFile profilepics)
        {
            var user = await _identityService.GetCurrentUser();
            var profilePics = await _profileService.ChangeProfilePics(user, profilepics);
            if(!profilePics.Status)
            {
                return BadRequest(new
                {
                    status = "Bad request",
                    message = profilePics.Message,
                    statusCode = 401
                });
            };
            return Ok(new
            {
                status = "success",
                message = "Registration Successfull",
                data = profilePics.Data
            });
        }

        [HttpGet("profilephoto")]
        public async Task<IActionResult> GetProfilePhoto()
        {
            var currentUser = await _identityService.GetCurrentUser();
            var profilePicUrlBaseResponse = await _profileService.GetProfilePic(currentUser);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "ProfilePics", profilePicUrlBaseResponse.Data!);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // Return 404 if file not found
            }
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileExtension = profilePicUrlBaseResponse.Data!.Split(".");
            return File(fileBytes, $"image/{fileExtension[1]}");
        }

        [Authorize]
        [HttpGet("p")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _identityService.GetCurrentUser();
            var profile = await _profileService.GetProfile(user.Id);
            if(profile == null)
            {
                return NotFound(new
                {
                    status = "Not Found",
                    Message = "Profile Not found",
                    StatusCode = 401 
                });
            }

            return Ok(new
            {
                status = "Successful",
                Message = "Profile found",
                Data = new
                {
                    firstName = profile.Data?.FirstName,
                    lastName = profile.Data?.LastName,
                    age = profile.Data?.DateOfBirth,
                    gender = profile.Data?.Gender,
                    profilePics = profile.Data?.ProfilePicture,
                    address = profile.Data?.Address,
                    occupation = profile.Data?.Occupation,
                    phoneNumber = profile.Data?.PhoneNumber,
                    nokFullName = profile.Data?.FullNameOfNextOfKin,
                    nokPhoneNumber = profile.Data?.ContactOfNextOfKin,
                    email = user.Email
                }
            });
        }

        [HttpGet("p/{userId}")]
        public async Task<IActionResult> GetProfile([FromQuery] Guid userId)
        {
            var profile = await _profileService.GetProfile(userId);
            if (profile == null)
            {
                return NotFound(new
                {
                    status = "Not Found",
                    Message = "Profile Not found",
                    StatusCode = 401
                });
            }

            return Ok(new
            {
                status = "Successful",
                Message = "Profile found",
                Data = new
                {
                    firstName = profile.Data?.FirstName,
                    lastName = profile.Data?.LastName,
                    age = profile.Data?.DateOfBirth,
                    gender = profile.Data?.Gender,
                    address = profile.Data?.Address,
                    occupation = profile.Data?.Occupation,
                    phoneNumber = profile.Data?.PhoneNumber,
                    nokFullName = profile.Data?.FullNameOfNextOfKin,
                    nokPhoneNumber = profile.Data?.ContactOfNextOfKin,
                }
            });
        }

        


    }
}
