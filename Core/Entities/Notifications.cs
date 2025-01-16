using AiComp.Domain.Entities;

namespace AiComp.Core.Entities
{
    public class Notification : BaseEntity
    {
        public string? description {  get; set; }
        public DateTime? TimeOfActivity { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
