using AiComp.Application.DTOs.RequestModel;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using AiComp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Embedding;

namespace AiComp.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class MoodController : ControllerBase
    {
        private readonly IMoodService _moodService;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;

        public MoodController(IMoodService moodService, IUserService userService, IIdentityService identityService)
        {
            _moodService = moodService;
            _userService = userService;
            _identityService = identityService;
        }

        [HttpGet("moods/all")]
        public async Task<IActionResult> GetAllMoodLogs()
        {
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                var moodLogs = await _moodService.ViewMoodLogs(currentUser);
                if (!moodLogs.Any())
                {
                    return NotFound();
                }
                return Ok(new
                {
                    Status = "Successful",
                    Message = $"{moodLogs.Count} mood found",
                    Data = moodLogs.Select(m => new
                    {
                        id = m.Id,
                        emotion = m.Emotion,
                        intensity = m.Intensity,
                        timestamp = m.Timestamp,
                        userId = m.UserId
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("moods/weekly")]
        public async Task<IActionResult> GetMoodLogsForAWeek()
        {
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                var today = DateTime.UtcNow;
                var weeklyMoodLog = await _moodService.ViewMoodLogsByTime(currentUser, today.AddDays(-7), today);
                if (!weeklyMoodLog.Any())
                {
                    return NotFound();
                }
                return Ok(new
                {
                    Status = "Success",
                    Message = "MoodLogs found",
                    Data = weeklyMoodLog.Select(m => new
                    {
                        m.Id,
                        m.Emotion,
                        m.Intensity,
                        m.Timestamp
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("moods/monthly")]
        public async Task<IActionResult> GetMoodLogsForTheMonth()
        {
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                var today = DateTime.UtcNow;
                var weeklyMoodLog = await _moodService.ViewMoodLogsByTime(currentUser, today.AddDays(-30), today);
                if (!weeklyMoodLog.Any())
                {
                    return NotFound();
                }
                return Ok(new
                {
                    Status = "Success",
                    Message = "MoodLogs found",
                    Data = weeklyMoodLog.Select(m => new
                    {
                        m.Id,
                        m.Emotion,
                        m.Intensity,
                        m.Timestamp
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("mood/Search")]
        public async Task<IActionResult> GetMoodLogsForDaysSpecifiedByUser([FromQuery] MoodSearchRequest request)
        { 
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                if(currentUser == null)
                {
                    return Unauthorized();
                }
                var logsBasedOnUserSearch = await _moodService.ViewMoodLogsByTime(currentUser, request.StartDate, request.EndDate);
                if (!logsBasedOnUserSearch.Any())
                {
                    return NotFound();
                }
                return Ok(new
                {
                    Status = "Success",
                    Message = "Mood(s) found",
                    Data = logsBasedOnUserSearch.Select(m => new
                    {
                        m.Id,
                        m.Emotion,
                        m.Intensity,
                        m.Timestamp
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
    }
}
