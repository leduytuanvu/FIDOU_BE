using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Entities
{
    [Table(nameof(ConversationSchedule))]
    public class ConversationSchedule
    {
        [Key]
        public Guid Id { get; set; }

        public Guid EnterpriseId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid OrderId { get; set; }

        public DateTime ScheduledTime { get; set; }


        [ForeignKey(nameof(EnterpriseId))]
        public Enterprise Enterprise { get; set; }

        [ForeignKey(nameof(CandidateId))]
        public Candidate Candidate { get; set; }
        
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
