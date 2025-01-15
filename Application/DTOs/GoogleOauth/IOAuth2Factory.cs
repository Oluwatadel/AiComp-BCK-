using AiComp.Application.Interfaces.Service;

namespace AiComp.Application.DTOs.GoogleOauth
{
    public interface IOAuth2Factory
    {
        IOAuthClient CreateOAuth2Client(string clientId);
    }
}
