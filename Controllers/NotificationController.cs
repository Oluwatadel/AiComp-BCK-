using AiComp.Application.Interfaces.Service;
using AiComp.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiComp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IIdentityService _identityService;

        public NotificationController(INotificationService notificationService, IIdentityService identityService)
        {
            _notificationService = notificationService;
            _identityService = identityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotification()
        {
            var currentUser = await _identityService.GetCurrentUser();
            if(currentUser == null)
            {
                return Unauthorized();
            }
            var userNotification = await _notificationService.GetAllNotificationsAsync(currentUser.Id);

            if(userNotification.Count == 0)
                return NoContent();

            var notificationList = userNotification.ToList();
            ICollection<Notification> returnNotification = new List<Notification>();

            //returnNotification = userNotification
            //.OrderByDescending(n => n.TimeOfActivity)
            //.Take(5)
            //.ToList()
            var count = Math.Max(0, userNotification.Count - 5);
            for(int i = userNotification.Count - 1; i >= count; i--)
            {
                returnNotification.Add(userNotification.ToList()[i]);
            }

            return Ok(new
            {
                data = returnNotification,
                message = "Sucess",
                status = "true"
            });
        }
    }
}
