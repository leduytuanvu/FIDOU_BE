using System;

namespace VoiceAPI.Models.Data.Notifications
{
    public class NotifyCandidateInvitedDataModel : NotifyBaseDataModel
    {
        public Guid JobId { get; set; }
        public Guid EnterpriseId { get; set; }
    }
}