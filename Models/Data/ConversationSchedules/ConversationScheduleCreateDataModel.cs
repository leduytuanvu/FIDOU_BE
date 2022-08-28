using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Data.ConversationSchedules
{
    public class ConversationScheduleCreateDataModel
    {
        public Guid EnterpriseId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid OrderId { get; set; }

        public DateTime ScheduledTime { get; set; }
    }
}
