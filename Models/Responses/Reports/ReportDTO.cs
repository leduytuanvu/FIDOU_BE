using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Responses.Reports
{
    public class ReportDTO
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string VoiceLink { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public bool IsReviewed { get; set; }

        public bool? IsTrue { get; set; }
        
        public Guid CandidateId { get; set; }
        
        public string CandidateEmail { get; set; }
        public string EnterpriseEmail { get; set; }
    }
}
