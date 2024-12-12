using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Repository
{
    public interface IProfileRepository
    {
        public Task<Profile> AddProfileAsync(Profile profile);
        public Task<Profile> UpdateProfileAsync(Profile profile);
        public Task<Profile> GetProfileAsync(Guid    userId);
    }
}
