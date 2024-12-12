using AiComp.Domain.Entities;
using System.Linq.Expressions;

namespace AiComp.Application.Interfaces.Repository
{
    public interface IMoodLogRepository
    {
        public Task<MoodLog> AddMoodLog(MoodLog moodLog);
        public Task<MoodLog> UpdateMoodLog(MoodLog moodLog);
        public Task<ICollection<MoodLog>> GetMoodLogsDynamically(Guid userId);
    }
}
