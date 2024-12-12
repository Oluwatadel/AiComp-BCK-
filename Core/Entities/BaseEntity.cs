namespace AiComp.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; private set; }
        private Guid GenerateID() => Id = Guid.NewGuid();
    }
}
