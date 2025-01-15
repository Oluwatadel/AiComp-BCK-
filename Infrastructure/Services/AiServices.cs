using AiComp.Application.Interfaces.Service;
using GroqSharp;
using GroqSharp.Models;
using AiComp.Application.Interfaces.Repository;
using AiComp.Domain.Entities;
using AiComp.Application.DTOs.ValueObjects;
using Google.Rpc;
using AiComp.Application.DTOs;
namespace AiComp.Infrastructure.Services
{
    public class AiServices : IAiServices
    {
        // State management: Keep track of user's current question
        private static Dictionary<string, int> userQuestionIndex = new Dictionary<string, int>();

        // Initial set of questions
        private static string[] questions = new string[]
        {
            "How are you feeling right now (e.g., happy, sad, angry)?",
            "What has been your dominant emotion today (e.g., calm, anxious, frustrated)?",
            "Can you describe your current energy level and motivation (e.g., energized, exhausted, neutral)?",
            "Is there anything currently bothering or exciting you (e.g., stressed, excited, relaxed)?"
        };

        private readonly IConversationRepository _converseRepository;
        private readonly IGroqClient _groq;
        private readonly IConfiguration _configuration;
        public AiServices(IConversationRepository converseRepository, IGroqClient groq, IConfiguration configuration)
        {
            _converseRepository = converseRepository;
            _groq = groq;
            _configuration = configuration;
        }

        public async Task<string> GetNextMoodQuestions(string question)
        {
            //switch made better
            var returnQuestion = question switch
            {
                "How are you feeling right now (e.g., happy, sad, angry)?" => questions[1],
                "What has been your dominant emotion today (e.g., calm, anxious, frustrated)?" => questions[2],
                "Can you describe your current energy level and motivation (e.g., energized, exhausted, neutral)?" => questions[3],
                "" => questions[0],
                _ => ""
            };
            return await Task.FromResult(returnQuestion);
        }

        public async Task<string> GetResponseFromAiOnDailyMood()
        {
            var userPromt = "You are an experienced psychologist (\"Emotion\": \"Your response\", \"Intensity\": \"your ratings on scale of 1 - 10\"))";
            var messages = new List<Message>()
            {
                new Message{Content = userPromt, Role = MessageRoleType.User },
                new Message{Content = "You are a professional Therapist, Psychiatrist and Psychologist", Role = MessageRoleType.System},
            };
            var response = await _groq.CreateChatCompletionAsync(messages.ToArray());
            return response;
        }

        public async Task<string> ChatCompletionAsync(List<MoodMessage> messages)
        {
            var messageArray = new List<Message>();

            foreach (var message in messages)
            {
                messageArray.Add(new Message { Content = message.Content, Role = message.Role });
            }
            messageArray.Add(new Message {Content = "You are a psychologist and a data analyst capable of mood analysis that responds in JSON.  The mood JSON schema should include\r\n{\r\n  \"Mood\": {\r\n    \"Emotion\": \"string (Happy, neutral, Sad)\",\r\n    \"Intensity\": \"number (1 - 10)\"\r\n  }\r\n}. Analyse the mood of user accurately from the conversation and give a cumulative response only in the json schema provided with only one object (Mood) (don't give any explanation or pre-introduction text. Just the Json response only)", Role = MessageRoleType.System});
            var response = await _groq.CreateChatCompletionAsync(messageArray.ToArray());
            return response;
        }
        
        public async Task<string> ChatCompletionAsync(string message)
        {
            var messageArray = new List<Message>
            {
                new Message { Content = $"Describe my current emotional state with this mood analysis {message} in one sentence", Role = MessageRoleType.System }
            };
            var response = await _groq.CreateChatCompletionAsync(messageArray.ToArray());
            return response;
        }

        public async Task<BaseResponse<string>> ChatCompletionStream(IEnumerable<ChatConverse> chats, string prompt)
        {
            try
            {
                var messageArray = new List<Message>
                {
                    new Message
                    {
                        Content = "You are a Companion and the same time a mental Companion who is watching out for red flags like sucidal word or depressive words",
                        Role = MessageRoleType.System
                    }
                };
                foreach (var chat in chats)
                {
                    if (string.IsNullOrWhiteSpace(chat.Prompt.ChatPromptToAi))
                    {
                        messageArray.Add(new Message { Content = chat.Prompt.ChatPromptToAi!, Role = MessageRoleType.User });
                    }
                    if (string.IsNullOrWhiteSpace(chat.Response.AiResponse))
                    {
                        messageArray.Add(new Message { Content = chat.Response.AiResponse!, Role = MessageRoleType.System });
                    }
                }
                messageArray.Add(new Message { Content = prompt, Role = MessageRoleType.User });

                string response = await _groq.CreateChatCompletionAsync(messageArray.ToArray());
                var baseResponse = new BaseResponse<string>();
                if(string.IsNullOrEmpty(response))
                {
                    baseResponse.SetValues("Error with GroqShap", false, null);
                    return baseResponse;
                }

                baseResponse.SetValues("Success", true, response);
                return baseResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ChatCompletionAsync: {ex.Message}");
            }

        }

        public async Task<BaseResponse<string>> ChatCompletionStream(string prompt)
        {
            try
            {
                var messageArray = new List<Message>
                {
                    new Message
                    {
                        Content = "You are a Companion and the same time a mental Companion who is watching out for red flags like sucidal word or depressive words",
                        Role = MessageRoleType.System
                    },
                    new Message
                    {
                        Content = prompt,
                        Role = MessageRoleType.User
                    }
                };

                string response = await _groq.CreateChatCompletionAsync(messageArray.ToArray());
                var baseResponse = new BaseResponse<string>();
                if (string.IsNullOrEmpty(response))
                {
                    baseResponse.SetValues("Error with GroqShap", false, null);
                    return baseResponse;
                }

                baseResponse.SetValues("Success", true, response);
                return baseResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ChatCompletionAsync: {ex.Message}");
            }

        }

        public async Task<string[]> GetAllQuestions()
        {
            return await Task.FromResult(questions);
        }

    }
}

