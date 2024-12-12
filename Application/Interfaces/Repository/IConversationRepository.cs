using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Repository
{
    public interface IConversationRepository
    {
        public Task<Conversation> AddConversationAsync(Conversation conversation);
        public Task<Conversation> GetConversationAync(Guid UserId);

    }
}
