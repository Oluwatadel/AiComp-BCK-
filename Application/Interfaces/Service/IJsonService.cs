using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface IJsonService
    {
        public Task<SentimentPrediction?> ConvertJsonStringToSentimentPrediction(string aiJson);
    }
}
