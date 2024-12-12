using AiComp.Application.DTOs.ValueObjects;
using AiComp.Application.Interfaces.Repository;
using AiComp.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class ChatConverseRepository : IChatConverseRepository
    {
        private readonly AiCompDBContext _dbContext;

        public ChatConverseRepository(AiCompDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<ChatConverse>> GetChatConverse(Guid conversationId)
        {
            var conversation = await _dbContext.Conversations.FirstOrDefaultAsync(a => a.Id == conversationId);
            var chats = conversation?.Conversations.ToList();
            return chats;
        }
    }
}
