using AiComp.Application.DTOs;
using AiComp.Core.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface IJournalService
    {
        public Task<BaseResponse<Journal>> AddJournalAsync(Guid userId, Journal journal);
        public Task<Journal> GetJournalAsync(Guid journalId);
        public Task<ICollection<Journal>> GetAllJournalsAsync(Guid UserId);
        public Task<int> DeleteJournal(Guid userId, Guid journalId);
        public Task<Journal> UpdateJournalAsync(Journal journal);
        public Task<bool> JournalExistWithTheTitleAsync(Journal journal);
    }
}
