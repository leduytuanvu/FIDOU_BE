using System;
using VoiceAPI.Entities;
using VoiceAPI.Entities.Enums;
using VoiceAPI.Models.Responses.Candidates;
using VoiceAPI.Models.Responses.Jobs;

namespace VoiceAPI.Models.Responses.Orders
{
    public class OrderHistoryDTO
    {
        public Guid Id { get; set; }

        public OrderStatusEnum Status { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        
        public CandidateDTO Candidate { get; set; }
        
        public JobDTO Job { get; set; }
    }
}