using AiComp.Domain.Entities;

namespace AiComp.Core.Entities
{
    public class Journal : BaseEntity
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime TimeCreate { get; set; } = DateTime.UtcNow;
        public User User { get; set; }
        public Guid UserId { get; set; }


    }
}
