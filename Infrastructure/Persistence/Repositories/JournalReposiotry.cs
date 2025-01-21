using AiComp.Application.Interfaces.Repository;
using AiComp.Core.Entities;
using AiComp.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class JournalRepository : IJournalRepository
    {
        private readonly AiCompDBContext _dbContext;

        public JournalRepository(AiCompDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Journal> AddJournalAsync(Journal journal)
        {
            await _dbContext.Journals.AddAsync(journal);
            return journal;
        }

        public void DeleteJournal(Journal journal)
        {
            _dbContext.Journals.Remove(journal);
        }

        public async Task<Journal> GetJournalAsync(Guid journalId)
        {
            return await _dbContext.Journals.FirstOrDefaultAsync(a => a.Id == journalId);
        }

        public async Task<Journal> UpdateJournalAsync(Journal journal)
        {
            _dbContext.Journals.Update(journal);
            return await Task.FromResult(journal);
        }

        public async Task<ICollection<Journal>> GetAllJournalsOfACertainUser(Guid userId)
        {
            var journals = await _dbContext.Journals.Where(a => a.UserId == userId).ToListAsync();
            return journals;
        }

        public Task<bool> JournalExist(string journalTitle)
        {
            return _dbContext.Journals.AnyAsync(a => a.Title == journalTitle);
        }
    }
}
