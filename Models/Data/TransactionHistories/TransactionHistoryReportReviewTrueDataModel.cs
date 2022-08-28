using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Data.TransactionHistories
{
    public class TransactionHistoryReportReviewTrueDataModel
    {
        public Guid JobId { get; set; }

        public Guid EnterpriseId { get; set; }

        public Guid CandidateId { get; set; }
    }
}
