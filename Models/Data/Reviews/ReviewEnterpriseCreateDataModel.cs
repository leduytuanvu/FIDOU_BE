using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Payload.Reviews;

namespace VoiceAPI.Models.Data.Reviews
{
    public class ReviewEnterpriseCreateDataModel
    {
        public Guid EnterpriseId { get; set; }
        public ReviewEnterpriseCreatePayload Payload { get; set; }
    }
}
