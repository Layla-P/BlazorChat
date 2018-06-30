using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorChat.Server.Services;
using BlazorChat.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Rest.Sync.V1;

namespace BlazorChat.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TwilioController : Controller
    {
        private readonly TwilioAccount _account;
        private readonly ITokenGenerator _tokenGenerator;
        public TwilioController(IOptions<TwilioAccount> account, ITokenGenerator tokenGenerator)
        {
            _account = account.Value;
            _tokenGenerator = tokenGenerator;

            if (_account.SyncServiceSid == String.Empty)
            {
                _account.SyncServiceSid = "default";
            }

            TwilioClient.Init(
                _account.ApiKey,
                _account.ApiSecret,
                _account.AccountSid
            );

            ProvisionSyncDefaultService(_account.SyncServiceSid);

        }

        [HttpGet("[action]")]
        public async Task<string> GetToken(string identity)
        {
            if ( identity == null)
            {
                identity = randomUserId();
            }

            var token = await 
                _tokenGenerator
                .Generate(identity)
                .ConfigureAwait(false);

            return token;
        }

        [HttpPost("[action]")]
        public async Task Create([FromBody]Chat chat)
        {
            Console.WriteLine(chat.author + " " + chat.body);
        }

        private void ProvisionSyncDefaultService(string serviceSid)
        {
            if (_account.SyncServiceSid.Equals("default"))
            {
                ServiceResource.Fetch("default");
            }
        }
        private string randomUserId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}