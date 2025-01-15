using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Repository
{
    public interface IMoodLogRepository
    {
        public Task<MoodLog> AddMoodLog(MoodLog moodLog);
        public Task<MoodLog> UpdateMoodLog(MoodLog moodLog);
        public Task<ICollection<MoodLog>> GetMoodLogsDynamically(Guid userId);
        public Task<MoodLog> GetAMoodLogDynamically(Guid userId, DateTime today);
    }
}
