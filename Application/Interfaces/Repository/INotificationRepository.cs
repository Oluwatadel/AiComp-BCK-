using AiComp.Core.Entities;

namespace AiComp.Application.Interfaces.Repository
{
    public interface INotificationRepository
    {
        public Task<Notification> AddNotificationAsync(Notification notification);
        public Task<Notification> GetNotification(Guid notificationId);
        public void DeleteNotification(Notification notification);
        public Task<Notification> UpdateNotification(Notification notification);
        public Task<ICollection<Notification>> GetAllNotifications(Guid userId);
    }
}
