using AiComp.Application.Interfaces.Repository;
using AiComp.Infrastructure.Persistence.Context;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AiCompDBContext _dbContext;

        public UnitOfWork(AiCompDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChanges()
        {
            int affectedRow = await _dbContext.SaveChangesAsync();
            return affectedRow;
        }
    }
}
