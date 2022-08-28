using System;

namespace VoiceAPI.Models.Responses.Notifications
{
    public class NotifyCandidateInvitedDTO : NotifyBaseDTO
    {
        public Guid JobId { get; set; }
        public Guid EnterpriseId { get; set; }
    }
}