using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities.Enums;

namespace VoiceAPI.Models.Payload.Orders
{
    public class OrderCandidateGetHistoryPayload
    {
        public OrderStatusEnum? Status { get; set; }
    }
}
