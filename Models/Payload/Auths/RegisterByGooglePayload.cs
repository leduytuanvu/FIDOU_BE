using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Payload.Accounts;

namespace VoiceAPI.Models.Payload.Auths
{
    public class RegisterByGooglePayload
    {
        public string AccessToken { get; set; }
        public AccountCreatePayload Account { get; set; }
    }
}
