using AiComp.Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace AiComp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoodMessagesController : ControllerBase
    {
        private readonly IIdentityService _identityservice;
        private readonly IMoodMessageService _moodmessageservice;

        public MoodMessagesController(IIdentityService identityservice, IMoodMessageService moodmessageservice)
        {
            _identityservice = identityservice;
            _moodmessageservice = moodmessageservice;
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMoodMessages([FromRoute] Guid id)
        {
            var changes = await _moodmessageservice.DeleteAllMoodMessages(id);
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
                    Id = p.Id,
                    date = p.TimeCreated,
                    role = p.Role,
                    messageContent = p.Content,
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetTodayMoodMessages()
        {
            var currentUser = await _identityservice.GetCurrentUser();
            var messages = await _moodmessageservice.GetMoodMessagesAsync(currentUser.Id);
            if (messages.Count == 0)
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
                data = messages.Where(a => a.TimeCreated == DateTime.UtcNow).Select(p => new
                {
                    Id = p.Id,
                    date = p.TimeCreated,
                    role = p.Role,
                    messageContent = p.Content,
                })
            });
        }

    }

}
