using AiComp.Application.DTOs;
using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Core.Entities;
using AiComp.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AiComp.Infrastructure.Services
{
    public class JournalService : IJournalService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;

        public JournalService(IJournalRepository journalRepository, IUnitOfWork unitOfWork, INotificationRepository notificationRepository)
        {
            _journalRepository = journalRepository;
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<Journal>> AddJournalAsync(Guid userId, Journal journal)
        {
            var baseResponse = new BaseResponse<Journal>();
            if(_journalRepository.JournalExist(journal.Title))
            {
                baseResponse.SetValues("Journal with the title already exist", false,null);
                return baseResponse;
            }
            await _journalRepository.AddJournalAsync(journal);
            

            var notification = new Notification
            {
                description = "You added a new Journal",
                UserId = userId
            };
            await _notificationRepository.AddNotificationAsync(notification);
            var changes = await _unitOfWork.SaveChanges();
            if (changes == 0)
            {
                baseResponse.SetValues("Problem saving Journal", false, null);
                return baseResponse;
            }
            baseResponse.SetValues("Success", true, journal);
            return baseResponse;
        }

        public async Task<int> DeleteJournal(Guid userId, Guid journalId)
        {
            var journal = await _journalRepository.GetJournalAsync(journalId);
            _journalRepository.DeleteJournal(journal);

            var change = await _unitOfWork.SaveChanges();
            if(change > 0)
            {
                var notification = new Notification
                {
                    description = "You added a new Journal",
                    UserId = userId
                };
                await _notificationRepository.AddNotificationAsync(notification);
            }
            return change;
            
        }

        public Task<ICollection<Journal>> GetAllJournalsAsync(Guid userId)
        {
            var journals = _journalRepository.GetAllJournalsOfACertainUser(userId);
            return journals;
        }

        public Task<Journal> GetJournalAsync(Guid journalId)
        {
            var journal = _journalRepository.GetJournalAsync(journalId);
            return journal;
        }

        public async Task<Journal> UpdateJournalAsync(Journal journal)
        {
            _journalRepository.UpdateJournalAsync(journal);
            return await Task.FromResult(journal);
        }
    }
}
