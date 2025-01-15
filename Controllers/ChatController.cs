using AiComp.Application.DTOs.RequestModel;
using AiComp.Application.DTOs.ValueObjects;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using GroqSharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace AiComp.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IAiServices _aiServices;
        private readonly IUserService _userService;
        private readonly IChatConverseService _chatConverseService;
        private readonly IConversationService _conversationService;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly IMoodMessageService _moodMessageService;
        private readonly IMoodService _moodService;
        private readonly IJsonService _jsonService;



        public ChatController(IConfiguration configuration, IAiServices aiServices, IUserService userService, IChatConverseService chatConverseService, IConversationService conversationService,
            IIdentityService identityService, IMoodMessageService moodMessageService, IMoodService moodService, IJsonService jsonService)
        {
            _configuration = configuration;
            _aiServices = aiServices;
            _userService = userService;
            _chatConverseService = chatConverseService;
            _conversationService = conversationService;
            _identityService = identityService;
            _moodMessageService = moodMessageService;
            _moodService = moodService;
            _jsonService = jsonService;
        }

        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestionsToAnalyseMood()
        {
            var currentUser = await _identityService.GetCurrentUser();
            var moodAnalysisQuestions = await _aiServices.GetAllQuestions();
            var moodMessages = await _moodMessageService.GetMoodMessagesAsync(currentUser.Id);

            var todaysMoodMessage = moodMessages.Where(message => message.TimeCreated.Date == DateTime.UtcNow.Date).ToList();
            var nextQuestion = "";


            if (todaysMoodMessage.Count() > 0)
            {
                if (todaysMoodMessage.Count < 8 && todaysMoodMessage?.LastOrDefault()?.Role == MessageRoleType.System)
                    return Ok(new
                    {
                        message = $"There is no response to {moodMessages.LastOrDefault().Content}",
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

                return Ok(new
                {
                    message = "question found",
                    status = true,
                    data = new
                    {
                        question = nextQuestion,
                        timestamp = DateTime.UtcNow
                    }
                });

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

            return Ok(new
            {
                message = "question found",
                status = true,
                data = nextQuestion
            });
        }

        [HttpPost("userresponse")]
        public async Task<IActionResult> ResponseToDailyMoodAnalysisQuestions([FromBody] string response)
        {
            if(string.IsNullOrWhiteSpace(response))
            {
                return BadRequest(new
                {
                    message = "Response cannot be empty",
                    status = "Unsuccessful",
                });
            }
            var currentUser = await _identityService.GetCurrentUser();
            var moodMessages = await _moodMessageService.GetMoodMessagesAsync(currentUser.Id);

            MoodMessage moodMessageAlreadyExisted = null;

            foreach(var message in moodMessages)
            {
                if (message == moodMessages.LastOrDefault() && message.Content.Contains(response)
                    && message.TimeCreated.Date == DateTime.Now.Date && message.Role == MessageRoleType.User)
                {
                    moodMessageAlreadyExisted = message;
                }
            }
            if (moodMessageAlreadyExisted != null)
            {
                return Ok(new
                {
                    message = "response given",
                    status = "Unsuccessful",
                });
            }
            var todaysMoodMessage = moodMessages.Where(message => message.TimeCreated.Date == DateTime.UtcNow.Date).ToList();
            if(todaysMoodMessage.Count() >= 8 && todaysMoodMessage[todaysMoodMessage.Count() -1].Role != MessageRoleType.System)
            {
                return Ok(new
                {
                    message = "Today's mood has been analysed! Come back tomorrow tomorrow",
                    status =  "Unsuccessful",
                });
            }
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
                message = "Mood message added successfully",
                data = new
                {
                    userResponse = returnMoodMessage.Content,
                    timestamp = returnMoodMessage.TimeCreated,
                }
            });
        }

        [HttpGet("analysemood")]
        public async Task<IActionResult> AnalyseUserMood()
        {
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                var moodForToday = _moodService.GetTodaysMoodLog(currentUser);
                var message = await _moodMessageService.GetMoodMessagesAsync(currentUser.Id);
                var todaysMoodMessage = message.Where(a => a.TimeCreated.Date == DateTime.Now.Date).ToList();
                if (!message.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Your mood has not been analysed today"
                    });
                }
                var lastMessage = todaysMoodMessage.LastOrDefault();
                if (string.IsNullOrEmpty(lastMessage.Content) || string.IsNullOrWhiteSpace(lastMessage.Content))
                {
                    var moodMessageToBeDeleted = todaysMoodMessage.LastOrDefault();
                    await _moodMessageService.DeleteMoodMessageAsync(moodMessageToBeDeleted.MoodMessageId );
                }
                if (todaysMoodMessage.Count > 8)
                    return Ok(new
                    {
                        status = false,
                        message = "Your mood has not been analysed today"
                    });
                if (todaysMoodMessage.Count < 8)
                    return BadRequest(new
                    {
                        status = false,
                        message = "Answer all questions before mood can be analysed accurately"
                    });
                var response = "";

                response = await _aiServices.ChatCompletionAsync(todaysMoodMessage);
                var responseToSentiment = await _jsonService.ConvertJsonStringToSentimentPrediction(response);
                
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
                var result = await _aiServices.ChatCompletionAsync(response);
                var newMoodMessage = new MoodMessage()
                {
                    MoodMessageId = Guid.NewGuid(),
                    Content = result,
                    Role = MessageRoleType.System,
                    UserId = currentUser.Id,
                };

                await _moodMessageService.AddMoodMessageAsync(newMoodMessage);

                return Ok(new
                {
                    message = dbResponseOnMoodAddition?.Message,
                    status = dbResponseOnMoodAddition?.Status,
                    data = result  
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


        [HttpGet("chats")]
        public async Task<IActionResult> GetCompanionConversationChats()
        {
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                var conversation = await _conversationService.GetConversationAsync(currentUser);
                if(conversation.Data == null)
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
                    status = "Successful",
                    message = conversation.Message,
                    data = conversations.Data!.Select(p => new
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

        [HttpPost("chatstream")]
        public async Task<IActionResult> StreamChatCompletionAsync([FromBody] PromptRequest request)
        {
            try
            {
                var currentUser = await _identityService.GetCurrentUser();
                var userConversation = await _conversationService.GetConversationAsync(currentUser);
                if(!userConversation.Status)
                {
                    var newConversation = new Conversation(currentUser.Id);
                    userConversation = await _conversationService.AddConversation(currentUser, newConversation);
                }
                var allUserChatWithCompanion = await _chatConverseService.GetChatConverses(userConversation.Data!.Id);

                var response = allUserChatWithCompanion.Data == null
                    ? await _aiServices.ChatCompletionStream(request.Prompt)
                    : await _aiServices.ChatCompletionStream(allUserChatWithCompanion.Data!.TakeLast(10), request.Prompt);

                var userPrompt = new Prompt(request.Prompt);

                if(!response.Status)
                {
                    return BadRequest(response.Message);
                }
                var aiResponse = new Response(response.Data);
                var newChat = await _chatConverseService.CreateChatConverse(userPrompt, aiResponse);
                await _conversationService.AddChatToConversation(userConversation.Data, newChat.Data);

                return Ok(response);


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        
    }
}
