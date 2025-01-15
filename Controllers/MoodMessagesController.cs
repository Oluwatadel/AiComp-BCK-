using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using AiComp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace AiComp.Controllers
{
    [Route("api/message")]
    [ApiController]
    [Authorize]

    public class MoodMessagesController : ControllerBase
    {
        private readonly IIdentityService _identityservice;
        private readonly IMoodMessageService _moodmessageservice;

        public MoodMessagesController(IIdentityService identityservice, IMoodMessageService moodmessageservice)
        {
            _identityservice = identityservice;
            _moodmessageservice = moodmessageservice;
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAllMoodMessages()
        {
            var currentUser = await _identityservice.GetCurrentUser();
            var changes = await _moodmessageservice.DeleteAllMoodMessagesAsync(currentUser.Id);
            if (changes == 0)
            {
                return NoContent();
            }

            return Ok(new
            {
                status = "Successfull!!",
                message = "Delete successful"
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMoodMessage([FromRoute] Guid id)
        {
            var changes = await _moodmessageservice.DeleteMoodMessageAsync(id);
            if(changes == 0)
            {
                return BadRequest(new
                {
                    status = "Unsuccessfull",
                    message = "Delete was not successful"
                });
            }
            return Ok(new
            {
                status = "Successful",
                message = "Mood message successfully deleted"
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMoodMessages()
        {
            var currentUser = await _identityservice.GetCurrentUser();
            var messages = await _moodmessageservice.GetMoodMessagesAsync(currentUser.Id);
            if(messages.Count == 0)
            {
                return Ok(new
                {
                    status = "Successful",
                    message = "No message found"
                });
            }

            return Ok(new
            {
                status = "Successful",
                message = $"{messages.Count} messages found",
                data = messages.Select(p => new
                {
                    Id = p.MoodMessageId,
                    date = p.TimeCreated,
                    role = p.Role,
                    messageContent = p.Content,
                })
            });
        }

        [HttpGet("moodmessages")]
        public async Task<IActionResult> GetTodayMoodMessages()
        {
            var currentUser = await _identityservice.GetCurrentUser();
            var messages = await _moodmessageservice.GetMoodMessagesAsync(currentUser.Id);
            if (messages != null && messages.Count() > 0)
            {
                if(string.IsNullOrEmpty(messages.LastOrDefault().Content) || string.IsNullOrWhiteSpace(messages.LastOrDefault().Content))
                    await _moodmessageservice.DeleteMoodMessageAsync(messages.LastOrDefault().MoodMessageId);
            }
            if (messages.Count == 0)
            {
                return Ok(new
                {
                    status = "Successful",
                    message = "No message found"
                });
            }
            var response = messages.Where(a => a.TimeCreated.Date == DateTime.UtcNow.Date).OrderBy(a => a.TimeCreated);
            return Ok(new
            {
                status = "Successful",
                message = $"{messages.Count} messages found",
                data = response.Select(p => new
                {
                    Id = p.MoodMessageId,
                    date = p.TimeCreated,
                    role = p.Role,
                    messageContent = p.Content,
                }).ToList()
            });
        }

    }

}
