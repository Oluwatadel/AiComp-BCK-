using AiComp.Application.DTOs;
using AiComp.Application.DTOs.ValueObjects;
using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;

namespace AiComp.Infrastructure.Services
{
    public class ChatConverseService : IChatConverseService
    {
        private readonly IChatConverseRepository _chatConverseRepository;

        public ChatConverseService(IChatConverseRepository chatConverseRepository)
        {
            _chatConverseRepository = chatConverseRepository;
        }

        public async Task<BaseResponse<ChatConverse>> CreateChatConverse(Prompt prompt, Response response)
        {
            var chatConverse = new ChatConverse(prompt, response);
            var baseResponse = new BaseResponse<ChatConverse>();
            baseResponse.SetValues("chatCOnverse Created Successfully", true, chatConverse);
            return await Task.FromResult(baseResponse);
        }

        public async Task<BaseResponse<IEnumerable<ChatConverse>>> GetChatConverses(Guid ConversationId)
        {
            var response = new BaseResponse<IEnumerable<ChatConverse>>();
            var conversations = await _chatConverseRepository.GetChatConverse(ConversationId);
            if (conversations.Count() == 0)
            {
                response.SetValues("No COnversation Yet!!!", false, null);
                return response;
            }

            response.SetValues($"Chat Found", true, conversations);
            return response;
        }
    }
}
