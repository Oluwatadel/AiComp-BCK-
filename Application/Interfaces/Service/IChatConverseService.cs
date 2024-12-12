using AiComp.Application.DTOs;
using AiComp.Application.DTOs.ValueObjects;

namespace AiComp.Application.Interfaces.Service
{
    public interface IChatConverseService
    {
        public Task<BaseResponse<ChatConverse>> CreateChatConverse(Prompt prompt, Response response);
        Task<BaseResponse<IEnumerable<ChatConverse>>> GetChatConverses(Guid ConversationId);
    }
}
