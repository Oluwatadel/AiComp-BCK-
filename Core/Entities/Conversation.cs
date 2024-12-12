using AiComp.Application.DTOs.ValueObjects;

namespace AiComp.Domain.Entities
{
    public class Conversation : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public DateTime TimeSpan { get; private set; }
        public List<ChatConverse> Conversations { get; private set; } = new List<ChatConverse>();


        public Conversation(Guid userId) 
        {
            UserId = userId;
            SetTimeStamp();
        }

        public void SetTimeStamp() => TimeSpan = DateTime.UtcNow;

        public void SetUserObject(User user)
        {
            User = user;
        }

        public void AddPromptAndResponse(ChatConverse chatConverse)
        {
            Conversations.Add(chatConverse);

        }
    }
}
