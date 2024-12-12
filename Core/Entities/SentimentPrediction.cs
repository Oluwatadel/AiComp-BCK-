using AiComp.Application.DTOs.ValueObjects;

namespace AiComp.Domain.Entities
{
    public class SentimentPrediction
    {
        public string? Emotion { get; set; }
        public float Intensity { get; set; }


        public void SetText(string emotion)
        {
            Emotion = emotion;
        }
    }
}