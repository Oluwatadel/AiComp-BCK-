using AiComp.Application.DTOs;
using AiComp.Application.DTOs.ValueObjects;
using AiComp.Controllers;
using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface IConversationService
    {
        public Task<BaseResponse<Conversation>> AddConversation(User user, Conversation conversation);
        public Task<BaseResponse<Conversation>> AddChatToConversation(Conversation Conversation, ChatConverse chat);
        Task<BaseResponse<Conversation>> GetConversationAsync(User user);
    }
}
