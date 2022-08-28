using System;

namespace VoiceAPI.Models.Data.Notifications
{
    public class NotifyConversationScheduleDataModel : NotifyBaseDataModel
    {
        public Guid EnterpriseId { get; set; }
        
        public DateTime ScheduledTime { get; set; } 
    }
}