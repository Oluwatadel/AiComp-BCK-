﻿using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using GroqSharp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace AiComp.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {`
        private readonly IAiServices _aiServices;
        private readonly IUserService _userService;
        private readonly IChatConverseService _chatConverseService;
        private readonly IConversationService _conversationService;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly IMoodMessageService _moodMessageService;
        private readonly IMoodService _moodService;



        public ChatController(IConfiguration configuration, IAiServices aiServices, IUserService userService, IChatConverseService chatConverseService, IConversationService conversationService,
            IIdentityService identityService, IMoodMessageService moodMessageService, IMoodService moodService)
        {
            _configuration = configuration;
            _aiServices = aiServices;
            _userService = userService;
            _chatConverseService = chatConverseService;
            _conversationService = conversationService;
            _identityService = identityService;
            _moodMessageService = moodMessageService;
            _moodService = moodService;
        }

        [HttpPost("questions")]
        public async Task<IActionResult> GetQuestionsToAnalyseMood()
        {
            var currentUser = await _identityService.GetCurrentUser();
            var moodAnalysisQuestions = await _aiServices.GetAllQuestions();
            var moodMessages = await _moodMessageService.GetMoodMessagesAsync(currentUser.Id);

            var todaysMoodMessage = moodMessages.Where(message => message.TimeCreated.Date == DateTime.UtcNow.Date).ToList();
            var nextQuestion = "";


            if (todaysMoodMessage.Count() > 0)
            {
                if (todaysMoodMessage.Count < 9 && todaysMoodMessage?.LastOrDefault()?.Role == MessageRoleType.System)
                    return Ok(new
                    {
                        message = $"There is no response to {moodMessages.LastOrDefault()}",
                        status = "Error",
                        data = moodMessages?.LastOrDefault()?.Content
                    });
                var moodQuestions = todaysMoodMessage?.Where(message => message.Role == MessageRoleType.System).ToList();

                if (moodQuestions?.Count() > 0 && moodQuestions.Count() < moodAnalysisQuestions.Length)
                {
                    nextQuestion = await _aiServices.GetNextMoodQuestions(moodQuestions.LastOrDefault()!.Content);
                }
                else if (moodQuestions?.Count() == moodAnalysisQuestions.Length)
                {
                    return Ok(new
                    {
                        message = "Mood Analysis done already! Come back tomorrow",
                        status = "Unsuccessful"
                    });
                }

                var moodMessage = new MoodMessage
                {
                    Content = nextQuestion,
                    Role = MessageRoleType.System,
                    User = currentUser,
                    UserId = currentUser.Id

                };
                var savedMoodMessage = await _moodMessageService.AddMoodMessageAsync(moodMessage);
                if (savedMoodMessage == null)
                {
                    return BadRequest(new
                    {
                        message = "Internal error; Mood message not saved",
                        status = "Unsuccessful"
                    });
                }

                return Ok(new { Question = nextQuestion });

            }

            nextQuestion = await _aiServices.GetNextMoodQuestions("");
            var moodMessageToBeAddedToDb = new MoodMessage
            {
                Content = nextQuestion,
                Role = MessageRoleType.System,
                User = currentUser,
                UserId = currentUser.Id

            };
            var addedMoodMessage = await _moodMessageService.AddMoodMessageAsync(moodMessageToBeAddedToDb);
            if (addedMoodMessage == null)
            {
                return BadRequest(new
                {
                    message = "Internal error; Mood message not saved",
                    status = "Unsuccessful"
                });
            }

            return Ok(new { Question = nextQuestion });
        }

        [HttpPost("user_response")]
        public async Task<IActionResult> ResponseToDailyMoodAnalysisQuestions([FromBody] string response)
        {
            var currentUser = await _identityService.GetCurrentUser();
            var moodMessage = new MoodMessage
            {
                Content = response,
                Role = MessageRoleType.User,
                UserId = currentUser.Id,
                User = currentUser
            };
            var returnMoodMessage = await _moodMessageService.AddMoodMessageAsync(moodMessage);
            if (returnMoodMessage == null) return BadRequest(new
            {
                status = "Unsuccessful",
                message = "internal error!!! Mood message not saved",
            });

            return Ok(new
            {
                status = "Successful",
                message = "Mood messge added successfully",
                Data = new
                {
                    userResponse = returnMoodMessage.Content,
                    timestamp = returnMoodMessage.TimeCreated,
                }
            });
        }

        [HttpGet("Analyse_Mood")]
        public async Task<IActionResult> GetMoodMessagesForTodayAndAnalyseUserMood()
        {
<<<<<<< HEAD

=======
            var currentUser = await _identityService.GetCurrentUser();
            var message = await _moodMessageService.GetMoodMessagesAsync(currentUser.Id);
            var todaysMoodMessage = message.Where(a => a.TimeCreated.Date == DateTime.Now.Date).ToList();
            if (message.Count == 0)
            {
                return BadRequest(new
                {
                    Status = "Not Found",
                    message = "Your mood has not been analysed today"
                });
            }
            var response = "";
>>>>>>> f88e35d7f228b6fc26e312de2b17a365fd0c8837
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                var message = await _moodMessageService.GetMoodMessagesAsync(currentUser.Id);
                var todaysMoodMessage = message.Where(a => a.TimeCreated.Date == DateTime.Now.Date).ToList();
                if (message.Count == 0)
                {
                    return BadRequest(new
                    {
                        Status = "Not Found",
                        message = "Your mood has not been analysed today"
                    });
                }
                var response = "";

                response = await _aiServices.ChatCompletionAsync(todaysMoodMessage);
                var responseToSentiment = await ConvertJsonStringToSentimentPrediction(response);
                var newMoodMessage = new MoodMessage()
                {
                    Content = response,
                    Role = MessageRoleType.System,
                    UserId = currentUser.Id,
                    User = currentUser
                };

                if (!todaysMoodMessage.Any(a => a.UserId == currentUser.Id))
                {
                    await _moodMessageService.AddMoodMessageAsync(newMoodMessage);

                }
                var dbResponseOnMoodAddition = await _moodService.AddMoodLog(currentUser, responseToSentiment!);

                if (dbResponseOnMoodAddition == null)
                {
                    return BadRequest(new
                    {
                        message = dbResponseOnMoodAddition?.Message,
                        status = dbResponseOnMoodAddition?.Status,
                        data = dbResponseOnMoodAddition?.Data
                    });
                }
                return Ok(new
                {
                    message = dbResponseOnMoodAddition?.Message,
                    status = dbResponseOnMoodAddition?.Status,
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Unsuccessful",
                    message = ex.Message,
                });
            }
        }


<<<<<<< HEAD
        [HttpGet("chats")]
        public async Task<IActionResult> GetCompanionConversationChats()
        {
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                var conversation = await _conversationService.GetConversationAsync(currentUser);
                if(conversation == null)
                {
                    var newConversation = await _conversationService.AddConversation(currentUser, new Conversation(currentUser.Id));
                    if(newConversation.Data == null)
                    {
                        return BadRequest(new
                        {
                            Status = "Not Successful",
                            Message = newConversation.Message
                        });
                    }
                }

                var conversations = await _chatConverseService.GetChatConverses(conversation.Data.Id);
                if(!conversations.Status)
                {
                    return NoContent();
                }
                return Ok(new
                {
                    Status = "Successful",
                    Message = conversation.Message,
                    Data = conversations.Data!.Select(p => new
                    {
                        p.Prompt,
                        p.Response,
                        p.TimeCreated
                    })
                });

            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost("chats")]
        public async Task<IActionResult> SendChatToCompanion([FromForm] string response)
        {
            try
            {
                var currentUser = await _identityService.GetCurrentUser();

                var response = await _ai

                var conversation = await _conversationService.GetConversationAsync(currentUser);

            }

        }



        //private async Task<User> GetCurrentUser()
        //{
        //    var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        //    var currentUser = await _userService.GetUserByIdAsync(currentUserId);
        //    return currentUser;
        //}

=======
        
>>>>>>> f88e35d7f228b6fc26e312de2b17a365fd0c8837
        private async Task<SentimentPrediction?> ConvertJsonStringToSentimentPrediction(string aiJson)
        {
            var refinedJson = await MoodObjRegexPatternMatch(aiJson);
            var returnedSentiment = new SentimentPrediction();
            using (var doc = JsonDocument.Parse(refinedJson))
            {
                JsonElement root = doc.RootElement;
                var sentiment = root.GetProperty("Mood");

                //Accessing the properties
                string emotion = sentiment.GetProperty("Emotion").GetString()!;
                int intensity = sentiment.GetProperty("Intensity").GetInt32();
                returnedSentiment.Intensity = intensity;
                returnedSentiment.SetText(emotion);
            }

            return await Task.FromResult(returnedSentiment);
        }

        private async Task<string?> MoodObjRegexPatternMatch(string aiJson)
        {
            string returnJson = "";
            string pattern = "\"Mood\":\\s*\\{[^}]*\\}";

            Match match = Regex.Match(aiJson, pattern);

            if (match.Success)
            {
                returnJson = $"{{{match.Value}}}";
                return await Task.FromResult(returnJson);
            }
            return await Task.FromResult(returnJson);
        }
    }
}