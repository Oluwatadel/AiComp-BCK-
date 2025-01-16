using AiComp.Application.Interfaces.Repository;
using AiComp.Application.Interfaces.Service;
using AiComp.Infrastructure.Persistence.Context;
using AiComp.Infrastructure.Persistence.Repositories;
using AiComp.Infrastructure.Services;
using GroqSharp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using Microsoft.OpenApi.Models;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //Add services to the container.
        builder.Services.AddDbContext<AiCompDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<MLContext>();
        builder.Services.AddHttpContextAccessor();
        var apiKey = builder.Configuration["ApiKey"];
        var apiModel = "gemma2-9b-it"; // or "roberta-base", "longformer-base-4096", or "t5-small", "gemma2-9b-it". "mixtral-8x7b-32768"; //"llama-3.1-70b-versatile";

        //registering GroClient as a singleton using factory method
        builder.Services.AddSingleton<IGroqClient>(gq =>
            new GroqClient(apiKey, apiModel)
            .SetTemperature(0.5)
            .SetMaxTokens(512)
            .SetTopP(1)
            .SetStop("NONE")
            .SetStructuredRetryPolicy(5));


        builder.Services.AddScoped<IProfilePicUpload, ProfilePicUpload>();
        builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
        builder.Services.AddScoped<IChatConverseRepository, ChatConverseRepository>();
        builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
        builder.Services.AddScoped<IMoodLogRepository, MoodLogRepository>();
        builder.Services.AddScoped<IMoodMessageRepository, MoodMessageRepository>(); 
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
        builder.Services.AddScoped<IJournalRepository, JournalRepository>();
        builder.Services.AddScoped<IAiServices, AiServices>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<ISentimentAnalysis, SentimentAnalysis>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IChatConverseService, ChatConverseService>();
        builder.Services.AddScoped<IConversationService, ConversationService>();
        builder.Services.AddScoped<IMoodMessageService, MoodMessageService>();
        builder.Services.AddScoped<IMoodService, MoodService>();
        builder.Services.AddScoped<IJsonService, JsonService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IJournalService, JournalService>();


        builder.Services.AddControllers();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //Validate user that generate token
                    ValidateAudience = true, //Validate the recipient of the token is authorize to receive
                    ValidateLifetime = true, //Check if token has not expiry and if sign in key is valid
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = (context) =>
                    {
                        return Task.CompletedTask;
                    },
                    OnForbidden = (context) =>
                    {
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = (context) =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Assignment", Version = "v1" });

            //configuring swagger to include security definitions
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter a valid Token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            //Configure swagger to use JWT bearer token authentication
            o.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            //.AllowCredentials());
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowAll");

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}