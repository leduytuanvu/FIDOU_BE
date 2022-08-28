using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Payload.Orders;

namespace VoiceAPI.Models.Data.Orders
{
    public class OrderCandidateGetHistoryDataModel
    {
        public OrderCandidateGetHistoryPayload Payload { get; set; }
        public Guid CandidateId { get; set; }
    }
}
