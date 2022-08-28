using System;

namespace VoiceAPI.Models.Responses.Notifications
{
    public class NotifyConversationScheduleDTO : NotifyBaseDTO
    {
        public Guid EnterpriseId { get; set; }
        public Guid CandidateId { get; set; }
        
        public DateTime ScheduledTime { get; set; } 
    }
}