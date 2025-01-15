using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using System.Net;

namespace AiComp.Infrastructure.Services
{
    public class GoogleAuthenticationService
    {
        private string[] scopes = { "https://www.googleapis.com/auth/gmail.readonly", "https://googleapis.com/auth/youtube" };

        private readonly IConfiguration _configuration;

        public GoogleAuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<string> GoogleAuthenticate()
        {
            try
            {
                var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = _configuration["Google:ClientId"],
                        ClientSecret = _configuration["Google:ClientSecret"]
                    },
                    scopes,
                    "user", // You can replace "user" with an identifier, such as Environment.UserName
                    CancellationToken.None
                );
                if (credentials.Token.IsExpired(credentials.Flow.Clock))
                {
                    await credentials.RefreshTokenAsync(CancellationToken.None);
                }

                //var oauthService = new Oauth2Service(new Google.Apis.Services.BaseClientService.Initializer()
                //{
                //    HttpClientInitializer = credentials,
                //    ApplicationName = "YourAppName"
                //});

                // Use the credentials for authorized requests
                return credentials.Token.AccessToken;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

