namespace AiComp.Application.Interfaces.Repository
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChanges();
    }
}
