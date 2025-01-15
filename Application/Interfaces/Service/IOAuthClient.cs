using AiComp.Core.Entities;

namespace AiComp.Application.Interfaces.Service
{
    public interface IOAuthClient
    {
        string BuildAuthorizationUrl(string scope, string state = null);
        Task<OAuthToken> ExchangeCodeForTokenAsync(string authorizationCode);
    }
}
