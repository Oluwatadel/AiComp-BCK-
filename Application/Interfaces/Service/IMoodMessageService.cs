using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface IMoodMessageService
    {
        public Task<MoodMessage> AddMoodMessageAsync(MoodMessage message);

        public Task<List<MoodMessage>> GetMoodMessagesAsync(Guid userId);
        public Task<int> DeleteAllMoodMessagesAsync(Guid userId);
        public Task<int> DeleteMoodMessageAsync(Guid moodMessageId);
    }
}
