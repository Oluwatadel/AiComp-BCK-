using AiComp.Core.Entities;
using System.Collections.Specialized;

namespace AiComp.Application.DTOs.GoogleOauth
{
    public class GoogleOauth2Token : OAuthToken
    {
        public string Token { get; }

        public int ExpirationTime { get; }

        public GoogleOauth2Token(NameValueCollection parseQuery)
        {
            Token = parseQuery[0];
            ExpirationTime = int.Parse(parseQuery.Get("expires_in"));
        }

        public override string ToString()
        {
            return "Token" + Token + "\nExpires in: " + ExpirationTime;
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode(); 
        }
    }
}
