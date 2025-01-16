using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Core.Entities;

namespace AiComp.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Notification> AddNotificationAsync(Guid userId, Notification notification)
        {
            await _notificationRepository.AddNotificationAsync(notification);
            var notifications = await _notificationRepository.GetAllNotifications(userId);
            if (notifications.Count == 10)
            {
                List<Notification> notificationList = notifications.ToList();
                _notificationRepository.DeleteNotification(notificationList[0]);
            }
            return await _unitOfWork.SaveChanges() == 0 ? null : notification;
        }

        public async Task<int> DeleteNotification(Guid notificationId)
        {
            var notification = await _notificationRepository.GetNotification(notificationId);
            _notificationRepository.DeleteNotification(notification);
            var change = await _unitOfWork.SaveChanges();
            return change;
        }

        public async Task<ICollection<Notification>> GetAllNotificationsAsync(Guid UserId)
        {
            var notifications = await _notificationRepository.GetAllNotifications(UserId);
            return notifications;
        }

        public Task<Notification> GetNotificationAsync(Guid notificationId)
        {
            throw new NotImplementedException();
        }

        public Task<Notification> UpdateNotificationAsync(Notification notification)
        {
            throw new NotImplementedException();
        }
    }
}
