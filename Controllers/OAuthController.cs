using AiComp.Application.Interfaces.Service;
using AiComp.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiComp.Controllers
{
    [Route("api/oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IOAuthClient _googleOAuthClient;

        public OAuthController(IOAuthClient googleOAuthClient)
        {
            _googleOAuthClient = googleOAuthClient;
        }

        [HttpGet("authorize")]
        public IActionResult Authorize()
        {
            string scope = "https://www.googleapis.com/auth/userinfo.profile";
            string authorizationUrl = _googleOAuthClient.BuildAuthorizationUrl(scope);

            return Redirect(authorizationUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code is missing.");
            }

            try
            {
                OAuthToken token = await _googleOAuthClient.ExchangeCodeForTokenAsync(code);
                return Ok(new
                {
                    AccessToken = token.AccessToken,
                    ExpiresIn = token.ExpiresIn,
                    RefreshToken = token.RefreshToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exchanging authorization code: {ex.Message}");
            }
        }

    }
}
