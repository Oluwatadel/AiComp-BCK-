using AiComp.Application.DTOs.ValueObjects;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using System.Text.Json;

namespace AiComp.Infrastructure.Services
{
    public class JsonService : IJsonService
    {
        public async Task<SentimentPrediction?> ConvertJsonStringToSentimentPrediction(string aiJson)
        {
            JsonElement jsonElement = await ConvertStringToJSon(aiJson);
            if (jsonElement.TryGetProperty("Mood", out JsonElement moodElement))
            {
                // Deserialize the extracted "Mood" object
                Mood userMood = JsonSerializer.Deserialize<Mood>(moodElement.GetRawText());

                var sentimentPrediction = new SentimentPrediction
                {
                    Intensity = userMood.Intensity
                };
                sentimentPrediction.SetText(userMood.Emotion);

                return sentimentPrediction;
            }
            else
            {
                Mood userMood = JsonSerializer.Deserialize<Mood>(jsonElement.GetRawText());
                var sentimentPrediction = new SentimentPrediction
                {
                    Intensity = userMood.Intensity
                };
                sentimentPrediction.SetText(userMood.Emotion);
                return sentimentPrediction;
            }
        }

        private async Task<JsonElement> ConvertStringToJSon(string aiJson)
        {
            if (IsValidJson(aiJson))
            {
                string cleanedJson = aiJson.Replace("```json\n", "").Replace("\n```", "").Trim();
                JsonDocument doc = JsonDocument.Parse(cleanedJson);
                return await Task.FromResult(doc.RootElement);
            }
            JsonDocument docJson = JsonDocument.Parse(aiJson);
            return await Task.FromResult(docJson.RootElement);
        }

        private bool IsValidJson(string jsonString)
        {
            return jsonString.StartsWith("```json") && jsonString.EndsWith("```") ? true : false;
        }
    }
}
