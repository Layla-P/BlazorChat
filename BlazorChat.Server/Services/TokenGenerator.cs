using BlazorChat.Shared;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio.Jwt.AccessToken;

namespace BlazorChat.Server.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        public TwilioAccount _account;
        public TokenGenerator(IOptions<TwilioAccount> account)
        {
            _account = account.Value;
        }
        public Task<string> Generate(string identity)
        {
            var grants = new HashSet<IGrant>();

            var videoGrant = new VideoGrant();

            grants.Add(videoGrant);

            if (_account.ChatServiceSid != string.Empty)
            {
                var chatGrant = new ChatGrant()
                {
                    ServiceSid = _account.ChatServiceSid,
                };

                grants.Add(chatGrant);
            }

            if (_account.SyncServiceSid != string.Empty)
            {
                var syncGrant = new SyncGrant()
                {
                    ServiceSid = _account.SyncServiceSid
                };

                grants.Add(syncGrant);
            }

            var token = new Token(
                _account.AccountSid,
                _account.ApiKey,
                _account.ApiSecret,
                identity,
                grants: grants
            ).ToJwt();

            return Task.FromResult(token);
        }
    }
}
