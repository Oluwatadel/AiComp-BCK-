using AiComp.Application.DTOs.ValueObjects;

namespace AiComp.Application.Interfaces.Repository
{
    public interface IChatConverseRepository
    {
        public Task<List<ChatConverse>> GetChatConverse(Guid conversationId);
    }
}
