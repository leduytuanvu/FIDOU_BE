using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Entities.Enums;
using VoiceAPI.Models.Payload.Orders;

namespace VoiceAPI.Models.Data.Orders
{
    public class OrderEnterpriseGetHistoryDataModel
    {
        public OrderEnterpriseGetHistoryPayload Payload { get; set; }
        public Guid EnterpriseID { get; set; }
    }
}
