using AiComp.Application.Interfaces.Repository;
using AiComp.Domain.Entities;
using AiComp.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AiComp.Infrastructure.Persistence.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AiCompDBContext _dbContext;

        public ProfileRepository(AiCompDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Profile> AddProfileAsync(Profile profile)
        {
            await _dbContext.Profiles.AddAsync(profile);
            return profile;
        }

        public async Task<Profile> GetProfileAsync(Guid userId)
        {
            var profile = await _dbContext.Profiles.FirstOrDefaultAsync(a => a.UserId == userId);
            return profile;
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            _dbContext.Profiles.Update(profile);
            return await Task.FromResult(profile);
        }
    }
}
