using AiComp.Application.Interfaces.Repository;
using AiComp.Domain.Entities;
using AiComp.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AiCompDBContext _dbContext;

        public ConversationRepository(AiCompDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Conversation> AddConversationAsync(Conversation conversation)
        {
            await _dbContext.Conversations.AddAsync(conversation);
            return conversation;
        }

        public async Task<Conversation> GetConversationAync(Guid userId)
        {
            var conversation = await _dbContext.Conversations.FirstOrDefaultAsync(a => a.UserId == userId);
            return conversation;
        }
    }
}
