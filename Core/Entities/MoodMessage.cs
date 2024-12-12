using GroqSharp.Models;

namespace AiComp.Domain.Entities
{
    public class MoodMessage : Message
    {
        public Guid Id { get; set; } = new Guid();
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
    }
}
