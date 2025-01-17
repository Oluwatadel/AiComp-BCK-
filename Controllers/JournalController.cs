using AiComp.Application.DTOs.RequestModel;
using AiComp.Application.Interfaces.Service;
using AiComp.Core.Entities;
using Google.Cloud.Dialogflow.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiComp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JournalController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IJournalService _journalService;

        public JournalController(IIdentityService identityService, IJournalService journalService)
        {
            _identityService = identityService;
            _journalService = journalService;
        }

        [HttpPost]
        public async Task<IActionResult> AddJournal([FromBody] JournalRequestModel journalRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var currentUser = await _identityService.GetCurrentUser();
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                var journal = new Journal
                {
                    Title = journalRequest.Title,
                    Content = journalRequest.Content,
                    UserId = currentUser.Id,
                };

                var SavedJournal = await _journalService.AddJournalAsync(currentUser.Id, journal);
                if (!SavedJournal.Status && SavedJournal.Data == null)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = SavedJournal.Message
                    });
                }

                return Ok(new
                {
                    data = journal,
                    status = true,
                    message = SavedJournal.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteJournal([FromQuery] Guid journalId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var currentUser = await _identityService.GetCurrentUser();
                if(currentUser == null)
                {
                    return BadRequest(ModelState);
                }
                var deleteChanges = await _journalService.DeleteJournal(currentUser.Id, journalId);
                if(deleteChanges == 0)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Journal not deleted"
                    });
                }
                return Ok(new
                {
                    status = true,
                    message = "Success"
                });
            }
            catch (Exception ex)
            {
                return  BadRequest(ex.Message);
            }

            

        }


        [HttpGet]
        public async Task<IActionResult> GetAllJournals()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var currentUser = await _identityService.GetCurrentUser();
                if (currentUser == null)
                {
                    return BadRequest(ModelState);
                }

                var journals = await _journalService.GetAllJournalsAsync(currentUser.Id);
                if (journals.Count() == 0)
                    return NoContent();
                return Ok(new
                {
                    status = true,
                    data = journals.Select(p => new
                    {
                        id = p.Id,
                        content = p.Content,
                        timestamp = p.TimeCreate
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
