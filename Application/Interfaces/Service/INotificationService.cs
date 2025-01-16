using AiComp.Core.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface INotificationService
    {
        public Task<Notification> AddNotificationAsync(Guid userId, Notification notification);
        public Task<Notification> GetNotificationAsync(Guid notificationId);
        public Task<ICollection<Notification>> GetAllNotificationsAsync(Guid userId);
        public Task<int> DeleteNotification(Guid notificationId);
        public Task<Notification> UpdateNotificationAsync(Notification notification);
    }
}
