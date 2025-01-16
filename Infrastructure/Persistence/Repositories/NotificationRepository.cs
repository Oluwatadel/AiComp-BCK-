using AiComp.Application.Interfaces.Repository;
using AiComp.Core.Entities;
using AiComp.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AiCompDBContext _dbContext;

        public NotificationRepository(AiCompDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            await _dbContext.Notifications.AddAsync(notification);
            return notification;
        }

        public void DeleteNotification(Notification notification)
        {
            _dbContext.Notifications.Remove(notification);
        }

        public async Task<Notification> GetNotification(Guid notificationId)
        {
            var journal = await _dbContext.Notifications.FirstOrDefaultAsync(a => a.Id == notificationId);
            return journal;
        }
        
        public async Task<ICollection<Notification>> GetAllNotifications(Guid userId)
        {
            var journals = await _dbContext.Notifications.Where(a => a.UserId == userId).ToListAsync();
            return journals;
        }

        public async Task<Notification> UpdateNotification(Notification notification)
        {
            _dbContext.Notifications.Update(notification);
            return await Task.FromResult(notification);
        }
    }
}
