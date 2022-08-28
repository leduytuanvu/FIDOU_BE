using System;

namespace VoiceAPI.Models.Data.TransactionHistories
{
    public class TransactionHistoryCandidateRejectInvitationDataModel
    {
        public Guid WalletId { get; set; }
        public Guid JobId { get; set; }

        public decimal Amount { get; set; }
    }
}