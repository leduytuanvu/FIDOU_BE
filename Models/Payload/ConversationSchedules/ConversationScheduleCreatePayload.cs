using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Payload.ConversationSchedules
{
    public class ConversationScheduleCreatePayload
    {
        [Required]
        public Guid CandidateId { get; set; }
        
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public DateTime ScheduledTime { get; set; }
    }
}
