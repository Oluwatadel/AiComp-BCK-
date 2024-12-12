using AiComp.Domain.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface ISentimentAnalysis
    {
        public Task<SentimentPrediction> PredictSentiment(string text);
        public Task TrainModelAsync();

    }
}
