
using AiComp.Domain.Entities;

namespace AiComp.Application.DTOs.ValueObjects
{
    public class ChatConverse : BaseEntity
    {
        public Response Response { get; private set; }
        public Prompt Prompt { get; private set; }
        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;

        public ChatConverse(Prompt prompt, Response response) 
        {
            Prompt = prompt;
            Response = response;
        }
    }
}
