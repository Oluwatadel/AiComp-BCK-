namespace AiComp.Core.Entities
{
    public class OAuthToken
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public int ExpiresIn { get; init; }
        public string TokenType { get; init; }

        public override string ToString()
        {
            return $"AccessToken: {AccessToken}, ExpiresIn: {ExpiresIn} seconds, RefreshToken: {RefreshToken}";
        }
    }
}
