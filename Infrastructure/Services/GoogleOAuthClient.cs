using AiComp.Application.Interfaces.Service;
using AiComp.Core.Entities;

public class GoogleOAuthClient : IOAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _authorizationEndpoint;
    private readonly string _tokenEndpoint;
    private readonly string _redirectUri;

    public GoogleOAuthClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var googleConfig = configuration.GetSection("OAuth:Google");
        _clientId = googleConfig["ClientId"];
        _clientSecret = googleConfig["ClientSecret"];
        _authorizationEndpoint = googleConfig["AuthorizationEndpoint"];
        _tokenEndpoint = googleConfig["TokenEndpoint"];
        _redirectUri = googleConfig["RedirectUri"];
    }

    public string BuildAuthorizationUrl(string scope, string state = null)
    {
        if (string.IsNullOrEmpty(scope)) throw new ArgumentNullException(nameof(scope));

        var queryParams = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "redirect_uri", _redirectUri },
            { "response_type", "code" },
            { "scope", scope },
            { "access_type", "offline" },
            { "prompt", "consent" }
        };

        if (!string.IsNullOrEmpty(state))
        {
            queryParams["state"] = state;
        }

        var query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        return $"{_authorizationEndpoint}?{query}";
    }

    public async Task<OAuthToken> ExchangeCodeForTokenAsync(string authorizationCode)
    {
        if (string.IsNullOrEmpty(authorizationCode)) throw new ArgumentNullException(nameof(authorizationCode));

        var requestBody = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "code", authorizationCode },
            { "redirect_uri", _redirectUri },
            { "grant_type", "authorization_code" }
        };

        var response = await _httpClient.PostAsync(_tokenEndpoint, new FormUrlEncodedContent(requestBody));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to retrieve token: {response.StatusCode} - {error}");
        }

        var token = await response.Content.ReadFromJsonAsync<OAuthToken>();
        if (token == null)
        {
            throw new InvalidOperationException("Failed to deserialize token response.");
        }

        return token;
    }
}
