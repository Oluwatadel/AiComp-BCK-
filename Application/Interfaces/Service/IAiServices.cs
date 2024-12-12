using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Service;
public interface IAiServices
{
    Task<string> GetNextMoodQuestions(string userPrompt);
    Task<string> GetResponseFromAiOnDailyMood();
    Task<string> ChatCompletionAsync(List<MoodMessage> messages);
    Task<string[]> GetAllQuestions();
}
