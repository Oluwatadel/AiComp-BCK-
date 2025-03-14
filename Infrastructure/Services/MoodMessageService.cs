﻿using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using Npgsql;

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
            try
            {
                var returnedMessage = await _repository.AddMoodMessages(message);
                var changes = await _unitOfWork.SaveChanges();
                if (changes == 0)
                {
                    return null;
                }
                return returnedMessage;
            }
            catch(Exception ex)
            {
                return null;
            }

        }

        public async Task<List<MoodMessage>> GetMoodMessagesAsync(Guid userId)
        {
            var messages = await _repository.GetMoodMessages(userId);
            return messages;
        }

        public async Task<int> DeleteAllMoodMessagesAsync(Guid userId)
        {

            var moodMessages = await _repository.GetMoodMessages(userId);
            foreach(var moodMessage in moodMessages)
            {
                _repository.Delete(moodMessage);
            }
            var changes = await _unitOfWork.SaveChanges();
            return changes;
        }

        public async Task<int> DeleteMoodMessageAsync(Guid moodMessageId)
        {
            var moodMessage = await _repository.GetMoodMessage(moodMessageId);
            _repository.Delete(moodMessage);
            var changes = await _unitOfWork.SaveChanges();
            return await Task.FromResult(changes);
        }

    }
    }
