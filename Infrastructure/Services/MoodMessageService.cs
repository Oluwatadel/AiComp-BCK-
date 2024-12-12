using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;

namespace AiComp.Infrastructure.Services
{
    public class MoodMessageService : IMoodMessageService
    {
        private readonly IMoodMessageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MoodMessageService(IMoodMessageRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<MoodMessage> AddMoodMessageAsync(MoodMessage message)
        {
            var returnedMessage = await _repository.AddMoodMessages(message);
            var changes = await _unitOfWork.SaveChanges();
            if(changes == 0)
            {
                return null;
            }
            return returnedMessage;
        }

        public async Task<List<MoodMessage>> GetMoodMessagesAsync(Guid userId)
        {
            var messages = await _repository.GetMoodMessages(userId);
            return messages;
        }

        public async Task<int> DeleteAllMoodMessages(Guid userId)
        {

            var moodMessages = await _repository.GetMoodMessages(userId);
            foreach(var moodMessage in moodMessages)
            {
                _repository.Delete(moodMessage);
            }
            var changes = await _unitOfWork.SaveChanges();
            return changes;
        }

    }
    }
