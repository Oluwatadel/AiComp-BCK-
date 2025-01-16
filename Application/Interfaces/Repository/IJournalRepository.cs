using AiComp.Core.Entities;

namespace AiComp.Application.Interfaces.Repository
{
    public interface IJournalRepository

    {
        public Task<Journal> AddJournalAsync(Journal journal);
        public Task<Journal> GetJournalAsync(Guid journalId);
        public void DeleteJournal(Journal journal);
        public Task<Journal> UpdateJournalAsync(Journal journal);
        public Task<ICollection<Journal>> GetAllJournalsOfACertainUser(Guid Id);
        public bool JournalExist(string journalTitle);
    }
}
