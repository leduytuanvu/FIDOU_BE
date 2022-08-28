using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Entities
{
    [Table(nameof(Order))]
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CandidateId { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public OrderStatusEnum Status { get; set; }

        public Guid JobId { get; set; }

        [ForeignKey(nameof(CandidateId))]
        public Candidate Candidate { get; set; }

        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; }

        public Review Review { get; set; }
        public Report Report { get; set; }
    }
}
