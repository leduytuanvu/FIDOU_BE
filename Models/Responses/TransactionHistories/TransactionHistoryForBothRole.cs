using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceAPI.Models.Responses.TransactionHistories
{
    public class TransactionHistoryForBothRole
    {
        public TransactionHistoryDTO EnterpriseTransactionHistory { get; set; }
        public TransactionHistoryDTO CandidateTransactionHistory { get; set; }
    }
}
