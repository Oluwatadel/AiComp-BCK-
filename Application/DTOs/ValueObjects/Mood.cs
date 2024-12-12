namespace AiComp.Application.DTOs.ValueObjects
{
    public class Mood
    {
        public string Emotion { get; private set; }
        public int Intensity { get; private set; }

        public Mood(string emotion, int intensity)
        {
            Emotion = emotion;
            Intensity = intensity;
        }
    }
}
