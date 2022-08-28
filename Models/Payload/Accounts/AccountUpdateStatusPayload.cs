using System;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Models.Payload.Accounts
{
    public class AccountUpdateStatusPayload
    {
        public Guid AccountId { get; set; }
        public AccountStatusEnum Status { get; set; }
    }
}