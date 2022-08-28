using System;

namespace VoiceAPI.Models.Data.Notifications
{
    public class NotifyJobHaveNewApplicantDataModel : NotifyBaseDataModel
    {
        public Guid JobId { get; set; }
        public Guid CandidateId { get; set; }
    }
}