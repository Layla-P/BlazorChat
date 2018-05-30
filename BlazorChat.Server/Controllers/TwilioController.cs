using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public TwilioAccount _account;
        public TwilioController(IOptions<TwilioAccount> account)
        {
            _account = account.Value;

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
        public JsonResult Token()
        {
            // This can be tracked internally by your web app.
            var identity = randomUserId();


            var grants = new HashSet<IGrant>();

            var videoGrant = new VideoGrant();

            grants.Add(videoGrant);

            if (_account.ChatServiceSid != String.Empty)
            {
                // Create a "grant" which enables a client to use IPM as a given user,
                // on a given device.
                var chatGrant = new ChatGrant()
                {
                    ServiceSid = _account.ChatServiceSid,
                };

                grants.Add(chatGrant);
            }

            if (_account.SyncServiceSid != String.Empty)
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

            return new JsonResult(new Dictionary<string, string>()
            {
                {"identity", identity},
                {"token", token}
            });
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