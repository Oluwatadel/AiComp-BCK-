using AiComp.Application.DTOs;
using AiComp.Application.DTOs.ValueObjects;
using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Controllers;
using AiComp.Domain.Entities;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AiComp.Infrastructure.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IChatConverseRepository _chatConverseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConversationService(IConversationRepository conversationRepository, IUnitOfWork unitOfWork, IChatConverseRepository chatConverseRepository)
        {
            _conversationRepository = conversationRepository;
            _unitOfWork = unitOfWork;
            _chatConverseRepository = chatConverseRepository;
        }

        public async Task<BaseResponse<Conversation>> AddChatToConversation(Conversation conversation, ChatConverse chat)
        {
            conversation.AddPromptAndResponse(chat);
            var response = new BaseResponse<Conversation>();
            var changes = await _unitOfWork.SaveChanges();
            if(changes == 0)
            {
                response.SetValues("Something went wrong, chat not added to Converstaion!!!", false, null);
                return response;
            }
            response.SetValues("Conversation added Successfully", true, conversation);
            return response;
        }

        public async Task<BaseResponse<Conversation>> AddConversation(User user, Conversation conversation)
        {
            user.AddConversation(conversation);
            var response = new BaseResponse<Conversation>();
            await _conversationRepository.AddConversationAsync(conversation);
            var changes = await _unitOfWork.SaveChanges();
            if (changes == 0)
            {
                response.SetValues("Something went wrong, conversation not added!!!", false, null);
                return response;
            }
            response.SetValues("Conversation added Successfully", true, conversation);
            return response;
        }

        public async Task<BaseResponse<Conversation>> GetConversationAsync(User user)
        {
            var conversation = await _conversationRepository.GetConversationAync(user.Id);
            var response = new BaseResponse<Conversation>();
            if (conversation == null)
            {
                response.SetValues("Converstaion not Found", false, null);
                return response;
            }
            response.SetValues("Conversation found", true, conversation);
            return response;
        }

        
    }
}
