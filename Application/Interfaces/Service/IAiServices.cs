using AiComp.Application.DTOs;
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
    Task<BaseResponse<string>> ChatCompletionStream(IEnumerable<ChatConverse> chats, string prompt);
    Task<BaseResponse<string>> ChatCompletionStream(string prompt);
}
