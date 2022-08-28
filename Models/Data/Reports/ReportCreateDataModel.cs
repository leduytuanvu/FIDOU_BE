using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAPI.Models.Payload.Reports;

namespace VoiceAPI.Models.Data.Reports
{
    public class ReportCreateDataModel
    {
        public Guid EnterpriseId { get; set; }

        public ReportCreatePayload Payload { get; set; }
    }
}
