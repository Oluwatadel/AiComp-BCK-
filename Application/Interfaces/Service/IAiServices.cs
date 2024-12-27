using AiComp.Application.DTOs.ValueObjects;
using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Service;
public interface IAiServices
{
    Task<string> GetNextMoodQuestions(string userPrompt);
    Task<string> GetResponseFromAiOnDailyMood();
    Task<string> ChatCompletionAsync(List<MoodMessage> messages);
    Task<string> ChatCompletionAsync(string messages);
    Task<string[]> GetAllQuestions();
    IAsyncEnumerable<string> ChatCompletionStream(IEnumerable<ChatConverse> chats, string prompt);
    IAsyncEnumerable<string> ChatCompletionStream(string prompt);
}
