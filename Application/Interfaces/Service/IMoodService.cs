using AiComp.Application.DTOs;
using AiComp.Application.Interfaces.Repository;
using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface IMoodService
    {

        public Task<BaseResponse<MoodLog>> AddMoodLog(User user, SentimentPrediction predict);

        public Task<BaseResponse<MoodLog>> UpdateMoodLog(User user, Guid moodLogId, DateTime today, SentimentPrediction prediction);

        public Task<ICollection<MoodLog>> ViewMoodLogs(User user);

        public Task<IEnumerable<MoodLog>> ViewMoodLogsByTime(User user, DateTime startDate, DateTime endDate);
    }
}
