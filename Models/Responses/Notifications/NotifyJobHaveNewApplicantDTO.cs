using System;

namespace VoiceAPI.Models.Responses.Notifications
{
    public class NotifyJobHaveNewApplicantDTO : NotifyBaseDTO
    {
        public Guid JobId { get; set; }
        public Guid EnterpriseId { get; set; }
    }
}