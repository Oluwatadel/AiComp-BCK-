using AiComp.Application.DTOs.RequestModel;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiComp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;

        public AuthenticationController(IUserService userService, IIdentityService identityService)
        {
            _userService = userService;
            _identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequestModel request)
        {
            try
            {
                var user = await _userService.UserExist(request.Email);
                if (user == true) return Conflict(new
                {
                    status = "Duplicate email",
                    message = "Duplicate Email",
                    statusCode = 409
                });
                var newUser = await _userService.AddUserAsync(request);
                return Created("", new
                {
                    status = "success",
                    message = "Registration Successfull",
                    data = new
                    {
                        newUser.Id,
                        newUser.Email,
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            try
            {
                var userExist = await _userService.UserExist(request.Email);
                if (!userExist) return StatusCode(401);
                var passWordCheck = await _identityService.AuthenticateUser(request.Email, request.Password);
                if (!passWordCheck)
                {
                    return StatusCode(401);
                }

                var user = await _userService.GetUserAsync(request.Email);
                var token = _identityService.GenerateToken(user);
                if (user.Profile == null) return Ok(new
                {
                    status = "Successfull",
                    message = "Login successfull",
                    data = new
                    {
                        accessToken = token,
                        user = new
                        {
                            user.Id,
                            user.Email,
                        },
                    },
                    user.Profile,

                });


                return Ok(new
                {
                    status = "Successfull",
                    message = "Login successfull",
                    data = new
                    {
                        accessToken = token,
                        user = new
                        {
                            user.Id,
                            user.Email,
                        },
                    },
                    profile = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("email/update")]
        public async Task<IActionResult> UpdateEmail([FromBody] string email)
        {
            User user = null;

            //User not logged In
            try
            {
                user = await _identityService.GetCurrentUser();
                var newUser = await _userService.UpdateUserEmailAsync(user.Id, email);

                //Internal server error
                if (newUser == null)
                {
                    return Unauthorized(new
                    {
                        message = "Something went wrong",
                        statusCode = 404,
                        status = "Unsuccessfull",

                    });
                }

                //Successfull update
                return Ok(new
                {
                    message = "Email updated succesfully",
                    statusCode = 200,
                    status = "Successfull"
                });
            }
            catch(Exception e)
            {
                return BadRequest(new
                {
                    message = "No user is logged in",
                    statusCode = 404,
                    status = "Unsuccessfull",

                });
            }
            
        }

        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                if (users.Count == 0)
                {
                    return NoContent();
                }

                return Ok(new
                {
                    message = $"{users.Count} users yet to register",
                    status = "Successful",
                    statusCode = 200,
                    Data = users.Select(p => new
                    {
                        email = p.Email,
                        isConsented = p.IsConsented,

                    })
                });
            }
            catch(Exception e)
            {
                return NotFound();
            }
        }

        

        //[HttpPost("LogOut")]
        //public async Task<IActionResult> LogOut()
        //{
        //    var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(currentUser)) return
        //            Unauthorized(new
        //            {
        //                status = "Unauthorised",
        //                Message = "User is Unauthorised",
        //                StatusCode = 401
        //            });
        //    var invalidateToken = 
        //}
    }
}
