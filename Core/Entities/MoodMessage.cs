using GroqSharp.Models;
using System.ComponentModel.DataAnnotations;

namespace AiComp.Domain.Entities
{
    public class MoodMessage : Message
    {
        [Key]
        public Guid MoodMessageId { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
    }
}
