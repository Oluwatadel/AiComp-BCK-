using AiComp.Application.Interfaces.Repository;
using AiComp.Domain.Entities;
using AiComp.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class MoodLogRepository : IMoodLogRepository
    {
        private readonly AiCompDBContext _aiCompDBContext;

        public MoodLogRepository(AiCompDBContext aiCompDBContext)
        {
            _aiCompDBContext = aiCompDBContext;
        }

        public async Task<MoodLog> AddMoodLog(MoodLog moodLog)
        {
            await _aiCompDBContext.MoodLogs.AddAsync(moodLog);
            return moodLog;
        }

        public async Task<ICollection<MoodLog>> GetMoodLogsDynamically(Guid userId)
        {
            return await _aiCompDBContext.MoodLogs.Where(a => a.UserId == userId).ToListAsync();
        }
        
        public async Task<MoodLog> GetAMoodLogDynamically(Guid userId, DateTime today)
        {
            return await _aiCompDBContext.MoodLogs.FirstOrDefaultAsync(a => a.Timestamp.Date == today.Date && a.UserId == userId);
        }

        public async Task<MoodLog> UpdateMoodLog(MoodLog moodLog)
        {
            await Task.FromResult(_aiCompDBContext.MoodLogs.Update(moodLog));
            return moodLog;
        }
    }
}
