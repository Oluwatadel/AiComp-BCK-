using AiComp.Application.DTOs.ValueObjects;

namespace AiComp.Domain.Entities
{
    public class MoodLog : BaseEntity
    {

        //Foreign Key
        public Guid UserId { get; set; }

        //Navigation property
        public User? User { get; private set; }
        public string? Emotion { get; private set; }
        public int Intensity { get; private set; }
        public DateTime Timestamp { get; private set; }


        public MoodLog(Guid userId)
        {
            UserId = userId;
            Timestamp = DateTime.UtcNow;
        }

        public void SetUserObject(User user)
        {
            User = user;
        }

        public void SetMood(SentimentPrediction sentiment)
        {
            Emotion = sentiment.Emotion;
            Intensity = (int)(sentiment.Intensity * 100); // Convert probability to a percentage for intensity
        }

        public void UpdateModeOfTheSameDay(SentimentPrediction sentiment)
        {
            Emotion = sentiment.Emotion;
            Intensity = (int)(sentiment.Intensity * 100);
        }

    }
}
