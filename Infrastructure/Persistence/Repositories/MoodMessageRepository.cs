using AiComp.Application.Interfaces.Repository;
using AiComp.Domain.Entities;
using AiComp.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class MoodMessageRepository : IMoodMessageRepository
    {
        private readonly AiCompDBContext _dbContext;

        public MoodMessageRepository(AiCompDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MoodMessage> AddMoodMessages(MoodMessage message)
        {
            await _dbContext.MoodMesages.AddAsync(message);
            return message;
        }

        public async Task<List<MoodMessage>> GetMoodMessages(Guid userId)
        {
            var moodMessages = _dbContext.MoodMesages.Where(m => m.UserId == userId);
            return await Task.FromResult(moodMessages.ToList());
        }

        public async Task<MoodMessage> GetMoodMessage(Guid moodMessageId)
        {
            var moodMessage = await _dbContext.MoodMesages.FirstOrDefaultAsync(m => m.MoodMessageId == moodMessageId);
            return moodMessage;
        }

        public void Delete(MoodMessage message)
        {
            _dbContext.MoodMesages.Remove(message);
        }



    }
}
